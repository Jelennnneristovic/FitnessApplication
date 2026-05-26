using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.Interfaces;
using UserServiceDomain.Entities;
using UserServiceInfrastructure.Data;

namespace UserServiceInfrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TrainerReview?> GetByIdAsync(Guid id)
        {
            return await _context.TrainerReviews
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<TrainerReview>> GetByTrainerIdAsync(Guid trainerId)
        {
            return await _context.TrainerReviews
                .Include(r => r.Client)
                .Where(r => r.TrainerId == trainerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TrainerReview>> GetByClientIdAsync(Guid clientId)
        {
            return await _context.TrainerReviews
                .Include(r => r.Client)
                .Where(r => r.ClientId == clientId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<(double average, int count)> GetTrainerRatingAsync(Guid trainerId)
        {
            var reviews = _context.TrainerReviews.Where(r => r.TrainerId == trainerId);

            var count = await reviews.CountAsync();
            if (count == 0)
                return (0, 0);

            var average = await reviews.AverageAsync(r => r.Rating);
            return (Math.Round(average, 2), count);
        }

        public async Task AddAsync(TrainerReview review)
        {
            await _context.TrainerReviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TrainerReview review)
        {
            _context.TrainerReviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
