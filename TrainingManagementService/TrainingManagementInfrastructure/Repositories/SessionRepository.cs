using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.Interfaces;
using TrainingManagementDomain.Entities;
using TrainingManagementInfrastructure.Data;

namespace TrainingManagementInfrastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly TrainingDbContext _context;

        public SessionRepository(TrainingDbContext context)
        {
            _context = context;
        }

        public async Task<TrainingSession?> GetByIdAsync(Guid id)
        {
            return await _context.TrainingSessions
                .Include(s => s.TrainingPlan)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<TrainingSession>> GetByPlanIdAsync(Guid planId)
        {
            return await _context.TrainingSessions
                .Include(s => s.TrainingPlan)
                .Where(s => s.TrainingPlanId == planId)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TrainingSession>> GetByTrainerIdAsync(Guid trainerId, DateTime? from, DateTime? to)
        {
            var query = _context.TrainingSessions
                .Include(s => s.TrainingPlan)
                .Where(s => s.TrainingPlan.TrainerId == trainerId);

            if (from.HasValue)
                query = query.Where(s => s.StartTime >= from.Value);

            if (to.HasValue)
                query = query.Where(s => s.StartTime <= to.Value);

            return await query.OrderBy(s => s.StartTime).ToListAsync();
        }

        public async Task<IEnumerable<TrainingSession>> GetByClientIdAsync(Guid clientId, DateTime? from, DateTime? to)
        {
            // Sve sesije iz planova na kojima je klijent Approved
            var query = _context.TrainingSessions
                .Include(s => s.TrainingPlan)
                .Where(s => s.TrainingPlan.Enrollments.Any(e =>
                    e.ClientId == clientId &&
                    e.Status == TrainingManagementDomain.Enums.EnrollmentStatus.Approved));

            if (from.HasValue)
                query = query.Where(s => s.StartTime >= from.Value);

            if (to.HasValue)
                query = query.Where(s => s.StartTime <= to.Value);

            return await query.OrderBy(s => s.StartTime).ToListAsync();
        }

        public async Task AddAsync(TrainingSession session)
        {
            await _context.TrainingSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TrainingSession session)
        {
            _context.TrainingSessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TrainingSession session)
        {
            _context.TrainingSessions.Remove(session);
            await _context.SaveChangesAsync();
        }
    }
}
