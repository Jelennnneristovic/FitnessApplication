using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.Interfaces;
using TrainingManagementDomain.Entities;
using TrainingManagementInfrastructure.Data;

namespace TrainingManagementInfrastructure.Repositories
{
    public class TrainingPlanRepository : ITrainingPlanRepository
    {
        private readonly TrainingDbContext _context;

        public TrainingPlanRepository(TrainingDbContext context)
        {
            _context = context;
        }

        public async Task<TrainingPlan?> GetByIdAsync(Guid id)
        {
            return await _context.TrainingPlans
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<TrainingPlan>> GetAllAsync(TrainingPlanFilterRequest filter)
        {
            var query = _context.TrainingPlans
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(keyword) ||
                    p.Description.ToLower().Contains(keyword));
            }

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.TrainerId.HasValue)
                query = query.Where(p => p.TrainerId == filter.TrainerId.Value);

            if (filter.Type.HasValue)
                query = query.Where(p => p.Type == filter.Type.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.Status.HasValue)
                query = query.Where(p => p.Status == filter.Status.Value);

            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<TrainingPlan>> GetByTrainerIdAsync(Guid trainerId)
        {
            return await _context.TrainingPlans
                .Include(p => p.Category)
                .Where(p => p.TrainerId == trainerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(TrainingPlan plan)
        {
            await _context.TrainingPlans.AddAsync(plan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TrainingPlan plan)
        {
            _context.TrainingPlans.Update(plan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TrainingPlan plan)
        {
            _context.TrainingPlans.Remove(plan);
            await _context.SaveChangesAsync();
        }
    }
}
