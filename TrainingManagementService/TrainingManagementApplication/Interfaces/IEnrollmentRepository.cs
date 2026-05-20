using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Entities;
using TrainingManagementDomain.Enums;

namespace TrainingManagementApplication.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetByIdAsync(Guid id);
        Task<Enrollment?> GetByClientAndPlanAsync(Guid clientId, Guid planId);
        Task<IEnumerable<Enrollment>> GetByClientIdAsync(Guid clientId, EnrollmentStatus? status);
        Task<IEnumerable<Enrollment>> GetByPlanIdAsync(Guid planId, EnrollmentStatus? status);
        Task<IEnumerable<Enrollment>> GetByTrainerIdAsync(Guid trainerId, EnrollmentStatus? status);
        Task<int> CountApprovedByPlanAsync(Guid planId);
        Task AddAsync(Enrollment enrollment);
        Task UpdateAsync(Enrollment enrollment);
    }
}
