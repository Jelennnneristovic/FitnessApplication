using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.DTOs.Responses;

namespace UserServiceApplication.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponse> CreateAsync(Guid trainerId, Guid clientId, string bearerToken, CreateReviewRequest request);
        Task<IEnumerable<ReviewResponse>> GetTrainerReviewsAsync(Guid trainerId);
        Task<TrainerRatingResponse> GetTrainerRatingAsync(Guid trainerId);
        Task<IEnumerable<ReviewResponse>> GetMyReviewsAsync(Guid clientId);
        Task DeleteAsync(Guid reviewId, Guid clientId);
    }
}
