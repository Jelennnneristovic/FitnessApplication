using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.Interfaces;
using UserServiceApplication.Services;
using UserServiceDomain.Entities;
using UserServiceDomain.Enums;

namespace UserServiceTest.Services
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITrainingServiceClient> _trainingServiceClientMock;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _trainingServiceClientMock = new Mock<ITrainingServiceClient>();

            _reviewService = new ReviewService(
                _reviewRepositoryMock.Object,
                _userRepositoryMock.Object,
                _trainingServiceClientMock.Object);
        }

        // === HELPER ===
        private static User CreateTrainer(Guid id) => new User
        {
            Id = id,
            Username = "trener",
            Email = "trener@test.com",
            FirstName = "Test",
            LastName = "Trener",
            Role = UserRole.Trainer,
            Status = UserStatus.Active
        };

        private const string FakeToken = "fake-jwt-token";

        // ===========================================
        // CreateAsync - USPESNI SLUCAJ
        // ===========================================

        [Fact]
        public async Task CreateAsync_WhenClientHasTrained_ShouldCreateReview()
        {
            // Arrange
            var trainerId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var trainer = CreateTrainer(trainerId);
            var request = new CreateReviewRequest { Rating = 5, Comment = "Odlican!" };

            // Trener postoji
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(trainerId))
                .ReturnsAsync(trainer);

            // Cross-service: klijent JE trenirao
            _trainingServiceClientMock
                .Setup(c => c.HasClientTrainedWithTrainerAsync(clientId, trainerId, FakeToken))
                .ReturnsAsync(true);

            // Posle AddAsync, GetByIdAsync vraca kreiranu recenziju
            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new TrainerReview
                {
                    Id = id,
                    TrainerId = trainerId,
                    ClientId = clientId,
                    Rating = 5,
                    Comment = "Odlican!",
                    CreatedAt = DateTime.UtcNow,
                    Client = new User { Id = clientId, Username = "klijent", FirstName = "Klijent", LastName = "Test" }
                });

            // Act
            var result = await _reviewService.CreateAsync(trainerId, clientId, FakeToken, request);

            // Assert
            result.Should().NotBeNull();
            result.Rating.Should().Be(5);
            result.Comment.Should().Be("Odlican!");

            // Provera da je review stvarno dodat
            _reviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainerReview>()), Times.Once);
        }

        // ===========================================
        // CreateAsync - CROSS-SERVICE PROVERA
        // ===========================================

        [Fact]
        public async Task CreateAsync_WhenClientHasNotTrained_ShouldThrowAndNotCreate()
        {
            // Arrange
            var trainerId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var trainer = CreateTrainer(trainerId);
            var request = new CreateReviewRequest { Rating = 5 };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(trainerId))
                .ReturnsAsync(trainer);

            // Cross-service: klijent NIJE trenirao
            _trainingServiceClientMock
                .Setup(c => c.HasClientTrainedWithTrainerAsync(clientId, trainerId, FakeToken))
                .ReturnsAsync(false);

            // Act & Assert
            var act = async () => await _reviewService.CreateAsync(trainerId, clientId, FakeToken, request);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*trenirali*");  // poruka sadrzi "trenirali"

            // KLJUCNO: review NIJE dodat jer provera nije prosla
            _reviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainerReview>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallTrainingServiceWithCorrectParameters()
        {
            // Arrange
            var trainerId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var trainer = CreateTrainer(trainerId);
            var request = new CreateReviewRequest { Rating = 4 };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(trainerId)).ReturnsAsync(trainer);
            _trainingServiceClientMock
                .Setup(c => c.HasClientTrainedWithTrainerAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new TrainerReview { Id = id, Client = new User() });

            // Act
            await _reviewService.CreateAsync(trainerId, clientId, FakeToken, request);

            // Assert - proveravamo da je cross-service pozvan SA PRAVIM argumentima
            _trainingServiceClientMock.Verify(
                c => c.HasClientTrainedWithTrainerAsync(clientId, trainerId, FakeToken),
                Times.Once);
        }

        // ===========================================
        // CreateAsync - VALIDACIJE
        // ===========================================

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-1)]
        [InlineData(100)]
        public async Task CreateAsync_WithInvalidRating_ShouldThrowArgumentException(int invalidRating)
        {
            // Arrange
            var trainerId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var request = new CreateReviewRequest { Rating = invalidRating };

            // Act & Assert
            var act = async () => await _reviewService.CreateAsync(trainerId, clientId, FakeToken, request);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*izmedju 1 i 5*");

            // Nije ni pozvao trening servis jer je validacija pukla prva
            _trainingServiceClientMock.Verify(
                c => c.HasClientTrainedWithTrainerAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenTrainerNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var trainerId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var request = new CreateReviewRequest { Rating = 5 };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(trainerId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var act = async () => await _reviewService.CreateAsync(trainerId, clientId, FakeToken, request);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_WhenTargetIsNotTrainer_ShouldThrowInvalidOperationException()
        {
            // Arrange - meta nije trener nego klijent
            var targetId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var notTrainer = new User
            {
                Id = targetId,
                Role = UserRole.Client,  // NIJE trener
                Status = UserStatus.Active
            };
            var request = new CreateReviewRequest { Rating = 5 };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(targetId))
                .ReturnsAsync(notTrainer);

            // Act & Assert
            var act = async () => await _reviewService.CreateAsync(targetId, clientId, FakeToken, request);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*samo trenere*");
        }

        [Fact]
        public async Task CreateAsync_WhenClientReviewsSelf_ShouldThrowInvalidOperationException()
        {
            // Arrange - klijent pokusava da oceni samog sebe (isti GUID)
            var sameId = Guid.NewGuid();
            var trainer = CreateTrainer(sameId);
            var request = new CreateReviewRequest { Rating = 5 };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(sameId))
                .ReturnsAsync(trainer);

            // Act & Assert
            var act = async () => await _reviewService.CreateAsync(sameId, sameId, FakeToken, request);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*sami sebe*");
        }

        // ===========================================
        // GetTrainerRatingAsync
        // ===========================================

        [Fact]
        public async Task GetTrainerRatingAsync_ShouldReturnAverageAndCount()
        {
            // Arrange
            var trainerId = Guid.NewGuid();
            _reviewRepositoryMock
                .Setup(r => r.GetTrainerRatingAsync(trainerId))
                .ReturnsAsync((4.5, 10));

            // Act
            var result = await _reviewService.GetTrainerRatingAsync(trainerId);

            // Assert
            result.AverageRating.Should().Be(4.5);
            result.TotalReviews.Should().Be(10);
            result.TrainerId.Should().Be(trainerId);
        }

        [Fact]
        public async Task GetTrainerRatingAsync_WithNoReviews_ShouldReturnZero()
        {
            // Arrange
            var trainerId = Guid.NewGuid();
            _reviewRepositoryMock
                .Setup(r => r.GetTrainerRatingAsync(trainerId))
                .ReturnsAsync((0, 0));

            // Act
            var result = await _reviewService.GetTrainerRatingAsync(trainerId);

            // Assert
            result.AverageRating.Should().Be(0);
            result.TotalReviews.Should().Be(0);
        }

        // ===========================================
        // DeleteAsync
        // ===========================================

        [Fact]
        public async Task DeleteAsync_WhenOwnReview_ShouldDelete()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var review = new TrainerReview
            {
                Id = reviewId,
                ClientId = clientId,  // ista osoba
                Client = new User()
            };

            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync(review);

            // Act
            await _reviewService.DeleteAsync(reviewId, clientId);

            // Assert
            _reviewRepositoryMock.Verify(r => r.DeleteAsync(review), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotOwnReview_ShouldThrowUnauthorized()
        {
            // Arrange - klijent pokusava da obrise tudju recenziju
            var reviewId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var otherClientId = Guid.NewGuid();
            var review = new TrainerReview
            {
                Id = reviewId,
                ClientId = ownerId,  // pripada nekom drugom
                Client = new User()
            };

            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync(review);

            // Act & Assert
            var act = async () => await _reviewService.DeleteAsync(reviewId, otherClientId);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();

            // Nije obrisano
            _reviewRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<TrainerReview>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenReviewNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var reviewId = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            _reviewRepositoryMock
                .Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync((TrainerReview?)null);

            // Act & Assert
            var act = async () => await _reviewService.DeleteAsync(reviewId, clientId);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}
