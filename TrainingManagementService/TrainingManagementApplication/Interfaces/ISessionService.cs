using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.DTOs.Responses;

namespace TrainingManagementApplication.Interfaces
{
    public interface ISessionService
    {
        // Trener
        Task<SessionResponse> CreateAsync(Guid trainerId, CreateSessionRequest request);
        Task<SessionResponse> UpdateAsync(Guid sessionId, Guid trainerId, UpdateSessionRequest request);
        Task DeleteAsync(Guid sessionId, Guid trainerId);
        Task<IEnumerable<SessionResponse>> GetByPlanAsync(Guid planId);
        Task<IEnumerable<SessionResponse>> GetMyTrainerScheduleAsync(Guid trainerId, DateTime? from, DateTime? to);

        // Klijent
        Task<IEnumerable<SessionResponse>> GetMyClientScheduleAsync(Guid clientId, DateTime? from, DateTime? to);

        // Zajednicki
        Task<SessionResponse> GetByIdAsync(Guid sessionId);
    }
}
