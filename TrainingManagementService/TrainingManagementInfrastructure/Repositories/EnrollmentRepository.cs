using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.Interfaces;
using TrainingManagementDomain.Entities;
using TrainingManagementDomain.Enums;
using TrainingManagementInfrastructure.Data;

namespace TrainingManagementInfrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly TrainingDbContext _context;

        public EnrollmentRepository(TrainingDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetByIdAsync(Guid id)
        {
            return await _context.Enrollments
                .Include(e => e.TrainingPlan)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Enrollment?> GetByClientAndPlanAsync(Guid clientId, Guid planId)
        {
            return await _context.Enrollments
                .Include(e => e.TrainingPlan)
                .FirstOrDefaultAsync(e =>
                    e.ClientId == clientId &&
                    e.TrainingPlanId == planId &&
                    (e.Status == EnrollmentStatus.Pending || e.Status == EnrollmentStatus.Approved));
        }

        public async Task<IEnumerable<Enrollment>> GetByClientIdAsync(Guid clientId, EnrollmentStatus? status)
        {
            var query = _context.Enrollments
                .Include(e => e.TrainingPlan)
                .Where(e => e.ClientId == clientId);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);

            return await query.OrderByDescending(e => e.RequestedAt).ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetByPlanIdAsync(Guid planId, EnrollmentStatus? status)
        {
            var query = _context.Enrollments
                .Include(e => e.TrainingPlan)
                .Where(e => e.TrainingPlanId == planId);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);

            return await query.OrderByDescending(e => e.RequestedAt).ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetByTrainerIdAsync(Guid trainerId, EnrollmentStatus? status)
        {
            var query = _context.Enrollments
                .Include(e => e.TrainingPlan)
                .Where(e => e.TrainingPlan.TrainerId == trainerId);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);

            return await query.OrderByDescending(e => e.RequestedAt).ToListAsync();
        }

        public async Task<int> CountApprovedByPlanAsync(Guid planId)
        {
            return await _context.Enrollments
                .CountAsync(e =>
                    e.TrainingPlanId == planId &&
                    e.Status == EnrollmentStatus.Approved);
        }

        public async Task AddAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasApprovedEnrollmentWithTrainerAsync(Guid clientId, Guid trainerId)
        {
            return await _context.Enrollments
                .Include(e => e.TrainingPlan)
                .AnyAsync(e =>
                    e.ClientId == clientId &&
                    e.Status == TrainingManagementDomain.Enums.EnrollmentStatus.Approved &&
                    e.TrainingPlan.TrainerId == trainerId);
        }
    }
}
