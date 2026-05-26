using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.DTOs.Responses;
using UserServiceApplication.Interfaces;
using UserServiceDomain.Entities;
using UserServiceDomain.Enums;

namespace UserServiceApplication.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITrainingServiceClient _trainingServiceClient;

        public ReviewService(
            IReviewRepository reviewRepository,
            IUserRepository userRepository,
            ITrainingServiceClient trainingServiceClient)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _trainingServiceClient = trainingServiceClient;
        }

        public async Task<ReviewResponse> CreateAsync(
            Guid trainerId, Guid clientId, string bearerToken, CreateReviewRequest request)
        {
            // Validacija ocene
            if (request.Rating < 1 || request.Rating > 5)
                throw new ArgumentException("Ocena mora biti izmedju 1 i 5.");

            // Provera da trener postoji i da je stvarno trener
            var trainer = await _userRepository.GetByIdAsync(trainerId)
                ?? throw new KeyNotFoundException("Trener nije pronadjen.");

            if (trainer.Role != UserRole.Trainer)
                throw new InvalidOperationException("Mozete oceniti samo trenere.");

            // Klijent ne moze oceniti sam sebe
            if (trainerId == clientId)
                throw new InvalidOperationException("Ne mozete oceniti sami sebe.");

            // === CROSS-SERVICE PROVERA ===
            // Pitamo TrainingService da li je klijent stvarno trenirao sa ovim trenerom
            var hasTrained = await _trainingServiceClient
                .HasClientTrainedWithTrainerAsync(clientId, trainerId, bearerToken);

            if (!hasTrained)
                throw new InvalidOperationException(
                    "Mozete oceniti samo trenere sa kojima ste trenirali.");

            // Kreiraj review
            var review = new TrainerReview
            {
                Id = Guid.NewGuid(),
                TrainerId = trainerId,
                ClientId = clientId,
                Rating = request.Rating,
                Comment = request.Comment?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);

            var created = await _reviewRepository.GetByIdAsync(review.Id);
            return MapToResponse(created!);
        }

        public async Task<IEnumerable<ReviewResponse>> GetTrainerReviewsAsync(Guid trainerId)
        {
            var reviews = await _reviewRepository.GetByTrainerIdAsync(trainerId);
            return reviews.Select(MapToResponse);
        }

        public async Task<TrainerRatingResponse> GetTrainerRatingAsync(Guid trainerId)
        {
            var (average, count) = await _reviewRepository.GetTrainerRatingAsync(trainerId);
            return new TrainerRatingResponse
            {
                TrainerId = trainerId,
                AverageRating = average,
                TotalReviews = count
            };
        }

        public async Task<IEnumerable<ReviewResponse>> GetMyReviewsAsync(Guid clientId)
        {
            var reviews = await _reviewRepository.GetByClientIdAsync(clientId);
            return reviews.Select(MapToResponse);
        }

        public async Task DeleteAsync(Guid reviewId, Guid clientId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId)
                ?? throw new KeyNotFoundException("Recenzija nije pronadjena.");

            // Klijent moze obrisati samo svoju recenziju
            if (review.ClientId != clientId)
                throw new UnauthorizedAccessException("Mozete obrisati samo svoju recenziju.");

            await _reviewRepository.DeleteAsync(review);
        }

        private static ReviewResponse MapToResponse(TrainerReview r)
        {
            return new ReviewResponse
            {
                Id = r.Id,
                TrainerId = r.TrainerId,
                ClientId = r.ClientId,
                ClientUsername = r.Client?.Username ?? string.Empty,
                ClientFirstName = r.Client?.FirstName ?? string.Empty,
                ClientLastName = r.Client?.LastName ?? string.Empty,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            };
        }
    }
}
