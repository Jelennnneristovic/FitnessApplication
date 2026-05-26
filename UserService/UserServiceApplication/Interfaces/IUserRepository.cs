using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceDomain.Entities;
using UserServiceDomain.Enums;
using DomainUser = UserServiceDomain.Entities.User;

namespace UserServiceApplication.Interfaces
{
    public interface IUserRepository
    {
        Task<DomainUser?> GetByIdAsync(Guid id);
        Task<DomainUser?> GetByUsernameAsync(string username);
        Task<DomainUser?> GetByEmailAsync(string email);
        Task<IEnumerable<DomainUser>> GetAllAsync();
        Task<IEnumerable<DomainUser>> GetByRoleAsync(UserRole role, string? keyword, UserStatus? status);
        Task AddAsync(DomainUser user);
        Task UpdateAsync(DomainUser user);


        // Trainer profile
        Task<TrainerProfile?> GetTrainerProfileByUserIdAsync(Guid userId);
        Task AddTrainerProfileAsync(TrainerProfile profile);
        Task UpdateTrainerProfileAsync(TrainerProfile profile);


        Task<IEnumerable<(User user, TrainerProfile? profile, double avgRating, int reviewCount)>>
         SearchTrainersAsync(string? keyword, string? specialization, double? minRating, string? sortBy);
    }
}
