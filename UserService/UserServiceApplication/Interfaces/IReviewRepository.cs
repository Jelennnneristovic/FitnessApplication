using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceDomain.Entities;

namespace UserServiceApplication.Interfaces
{
    public interface IReviewRepository
    {
        Task<TrainerReview?> GetByIdAsync(Guid id);
        Task<IEnumerable<TrainerReview>> GetByTrainerIdAsync(Guid trainerId);
        Task<IEnumerable<TrainerReview>> GetByClientIdAsync(Guid clientId);
        Task<(double average, int count)> GetTrainerRatingAsync(Guid trainerId);
        Task AddAsync(TrainerReview review);
        Task DeleteAsync(TrainerReview review);
    }
}
