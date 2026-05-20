using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementDomain.Entities;

namespace TrainingManagementApplication.Interfaces
{
    public interface ITrainingPlanRepository
    {
        Task<TrainingPlan?> GetByIdAsync(Guid id);
        Task<IEnumerable<TrainingPlan>> GetAllAsync(TrainingPlanFilterRequest filter);
        Task<IEnumerable<TrainingPlan>> GetByTrainerIdAsync(Guid trainerId);
        Task AddAsync(TrainingPlan plan);
        Task UpdateAsync(TrainingPlan plan);
        Task DeleteAsync(TrainingPlan plan);
    }
}
