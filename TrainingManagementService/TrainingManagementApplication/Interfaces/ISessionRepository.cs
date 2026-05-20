using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Entities;

namespace TrainingManagementApplication.Interfaces
{

    public interface ISessionRepository
    {
        Task<TrainingSession?> GetByIdAsync(Guid id);
        Task<IEnumerable<TrainingSession>> GetByPlanIdAsync(Guid planId);
        Task<IEnumerable<TrainingSession>> GetByTrainerIdAsync(Guid trainerId, DateTime? from, DateTime? to);
        Task<IEnumerable<TrainingSession>> GetByClientIdAsync(Guid clientId, DateTime? from, DateTime? to);
        Task AddAsync(TrainingSession session);
        Task UpdateAsync(TrainingSession session);
        Task DeleteAsync(TrainingSession session);
    }
}
