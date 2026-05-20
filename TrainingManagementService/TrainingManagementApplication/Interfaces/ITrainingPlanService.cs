using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.DTOs.Responses;

namespace TrainingManagementApplication.Interfaces
{
    public interface ITrainingPlanService
    {
        Task<IEnumerable<TrainingPlanResponse>> GetAllAsync(TrainingPlanFilterRequest filter);
        Task<TrainingPlanResponse> GetByIdAsync(Guid id);
        Task<IEnumerable<TrainingPlanResponse>> GetMyPlansAsync(Guid trainerId);
        Task<TrainingPlanResponse> CreateAsync(Guid trainerId, CreateTrainingPlanRequest request);
        Task<TrainingPlanResponse> UpdateAsync(Guid id, Guid trainerId, UpdateTrainingPlanRequest request);
        Task DeleteAsync(Guid id, Guid trainerId);

        //trainerId se prosleđuje u service kao parametar — service proverava da li trener pokušava da menja svoj plan ili tuđi.//


    }
}
