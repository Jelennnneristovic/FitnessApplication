using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.DTOs.Responses;
using TrainingManagementDomain.Enums;

namespace TrainingManagementApplication.Interfaces
{
    public interface IEnrollmentService
    {
        // Klijent
        Task<EnrollmentResponse> RequestEnrollmentAsync(Guid clientId, CreateEnrollmentRequest request);
        Task<IEnumerable<EnrollmentResponse>> GetMyEnrollmentsAsync(Guid clientId, EnrollmentStatus? status);
        Task CancelMyEnrollmentAsync(Guid enrollmentId, Guid clientId);

        // Trener
        Task<IEnumerable<EnrollmentResponse>> GetEnrollmentsForMyPlansAsync(Guid trainerId, EnrollmentStatus? status);
        Task<IEnumerable<EnrollmentResponse>> GetEnrollmentsByPlanAsync(Guid planId, Guid trainerId, EnrollmentStatus? status);
        Task<EnrollmentResponse> ApproveAsync(Guid enrollmentId, Guid trainerId, string bearerToken);
        Task<EnrollmentResponse> RejectAsync(Guid enrollmentId, Guid trainerId, string bearerToken, RejectEnrollmentRequest request);
        Task RemoveClientFromPlanAsync(Guid enrollmentId, Guid trainerId);
    }
}
