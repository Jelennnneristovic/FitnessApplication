using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserServiceApplication.Interfaces;
using UserServiceDomain.Entities;
using UserServiceDomain.Enums;
using UserServiceInfrastructure.Data;

namespace UserServiceInfrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, string? keyword, UserStatus? status)
        {
            var query = _context.Users.Where(u => u.Role == role);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var loweredKeyword = keyword.ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(loweredKeyword) ||
                    u.Email.ToLower().Contains(loweredKeyword) ||
                    u.FirstName.ToLower().Contains(loweredKeyword) ||
                    u.LastName.ToLower().Contains(loweredKeyword));
            }

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<TrainerProfile?> GetTrainerProfileByUserIdAsync(Guid userId)
        {
            return await _context.TrainerProfiles
                .Include(tp => tp.User)
                .FirstOrDefaultAsync(tp => tp.UserId == userId);
        }

        public async Task AddTrainerProfileAsync(TrainerProfile profile)
        {
            await _context.TrainerProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTrainerProfileAsync(TrainerProfile profile)
        {
            _context.TrainerProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }

    }
}
