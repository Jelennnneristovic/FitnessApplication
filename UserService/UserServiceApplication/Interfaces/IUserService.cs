using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.DTOs.Responses;

namespace UserServiceApplication.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllClientsAsync(UserFilterRequest filter);
        Task<IEnumerable<UserDTO>> GetAllTrainersAsync(UserFilterRequest filter);
        Task<UserDTO> ActivateAsync(Guid userId);
        Task<UserDTO> DeactivateAsync(Guid userId);
        Task<UserDetailsDTO> GetByIdAsync(Guid userId);
        Task<UserDetailsDTO> UpdateAsync(Guid userId, UpdateUserRequest request);
        Task<TrainerProfileResponse> GetTrainerProfileAsync(Guid userId);
        Task<TrainerProfileResponse> UpdateTrainerProfileAsync(Guid userId, UpdateTrainerProfileRequest request);

        Task<IEnumerable<TrainerSearchResponse>> SearchTrainersAsync(TrainerSearchRequest request);
    }
}
