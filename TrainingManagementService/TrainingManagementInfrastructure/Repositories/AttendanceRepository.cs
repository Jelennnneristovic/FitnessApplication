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
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly TrainingDbContext _context;

        public AttendanceRepository(TrainingDbContext context)
        {
            _context = context;
        }

        public async Task<Attendance?> GetByIdAsync(Guid id)
        {
            return await _context.Attendances
                .Include(a => a.TrainingSession)
                    .ThenInclude(s => s.TrainingPlan)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Attendance?> GetBySessionAndClientAsync(Guid sessionId, Guid clientId)
        {
            return await _context.Attendances
                .Include(a => a.TrainingSession)
                    .ThenInclude(s => s.TrainingPlan)
                .FirstOrDefaultAsync(a =>
                    a.TrainingSessionId == sessionId &&
                    a.ClientId == clientId);
        }

        public async Task<IEnumerable<Attendance>> GetByClientIdAsync(Guid clientId)
        {
            return await _context.Attendances
                .Include(a => a.TrainingSession)
                    .ThenInclude(s => s.TrainingPlan)
                .Where(a => a.ClientId == clientId)
                .OrderByDescending(a => a.TrainingSession.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetBySessionIdAsync(Guid sessionId)
        {
            return await _context.Attendances
                .Include(a => a.TrainingSession)
                    .ThenInclude(s => s.TrainingPlan)
                .Where(a => a.TrainingSessionId == sessionId)
                .ToListAsync();
        }

        public async Task AddAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
        }
    }
}
