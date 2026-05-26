using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.DTOs.Responses;
using UserServiceApplication.Interfaces;
using UserServiceDomain.Entities;
using UserServiceDomain.Enums;

namespace UserServiceApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDTO>> GetAllClientsAsync(UserFilterRequest filter)
        {
            var clients = await _userRepository.GetByRoleAsync(UserRole.Client, filter.Keyword,
                filter.Status);
            return clients.Select(MapToResponse);
        }

        public async Task<IEnumerable<UserDTO>> GetAllTrainersAsync(UserFilterRequest filter)
        {
            var trainers = await _userRepository.GetByRoleAsync(UserRole.Trainer, filter.Keyword,
                filter.Status);
            return trainers.Select(MapToResponse);
        }

        private static UserDTO MapToResponse(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Status = user.Status,
                Role = user.Role,
                ProfileImageUrl = user.ProfileImageUrl


            };
        }

        public async Task<UserDTO> ActivateAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
         ?? throw new KeyNotFoundException("Korisnik nije pronadjen.");

            if (user.Role == UserRole.Admin)
                throw new InvalidOperationException("Admin nalog ne moze biti menjan.");

            if (user.Status == UserStatus.Active)
                throw new InvalidOperationException("Korisnik je vec aktivan.");

            user.Status = UserStatus.Active;
            await _userRepository.UpdateAsync(user);

            return MapToResponse(user);
        }

        public async Task<UserDTO> DeactivateAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
        ?? throw new KeyNotFoundException("Korisnik nije pronadjen.");

            if (user.Role == UserRole.Admin)
                throw new InvalidOperationException("Admin nalog ne moze biti deaktiviran.");

            if (user.Status == UserStatus.InActive)
                throw new InvalidOperationException("Korisnik je vec deaktiviran.");

            user.Status = UserStatus.InActive;
            await _userRepository.UpdateAsync(user);

            return MapToResponse(user);
        }

        private static UserDetailsDTO MapToDetailsDTO(User user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            Role = user.Role,
            Location = user.Location,
            Status = user.Status,
            RegistrationDate = user.RegistrationDate,
            ProfileImageUrl = user.ProfileImageUrl
        }; 
        public async Task<UserDetailsDTO> GetByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            return MapToDetailsDTO(user);
        }
        public async Task<UserDetailsDTO> UpdateAsync(Guid userId, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            if (request.FirstName != null)
            {
                if (string.IsNullOrWhiteSpace(request.FirstName))
                    throw new ArgumentException("Ime ne moze biti prazno.");
                user.FirstName = request.FirstName.Trim();
            }

            // LastName
            if (request.LastName != null)
            {
                if (string.IsNullOrWhiteSpace(request.LastName))
                    throw new ArgumentException("Prezime ne moze biti prazno.");
                user.LastName = request.LastName.Trim();
            }

            // DateOfBirth
            if (request.DateOfBirth.HasValue)
            {
                if (request.DateOfBirth.Value > DateOnly.FromDateTime(DateTime.UtcNow))
                    throw new ArgumentException("Datum rodjenja ne moze biti u buducnosti.");

                var age = DateTime.UtcNow.Year - request.DateOfBirth.Value.Year;
                if (age < 13 || age > 120)
                    throw new ArgumentException("Korisnik mora imati izmedju 13 i 120 godina.");

                user.DateOfBirth = request.DateOfBirth.Value;
            }

            // Gender
            if (request.Gender.HasValue)
            {
                user.Gender = request.Gender.Value;
            }

            // Location
            if (request.Location != null)
            {
                user.Location = request.Location.Trim();
            }

            await _userRepository.UpdateAsync(user);

            return MapToDetailsDTO(user);
        }
        public async Task<TrainerProfileResponse> GetTrainerProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("Korisnik nije pronadjen.");

            if (user.Role != UserRole.Trainer)
                throw new InvalidOperationException("Korisnik nije trener.");

            var profile = await _userRepository.GetTrainerProfileByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Trenerski profil nije pronadjen.");

            return MapToProfileResponse(profile);
        }

        public async Task<TrainerProfileResponse> UpdateTrainerProfileAsync(Guid userId, UpdateTrainerProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("Korisnik nije pronadjen.");

            if (user.Role != UserRole.Trainer)
                throw new InvalidOperationException("Samo treneri mogu imati profil.");

            var profile = await _userRepository.GetTrainerProfileByUserIdAsync(userId);

            // Ako profil ne postoji (npr. trener registrovan pre nego sto smo dodali ovu funkcionalnost), kreiraj ga
            if (profile == null)
            {
                profile = new TrainerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = userId
                };
                await _userRepository.AddTrainerProfileAsync(profile);
            }

            // Selektivne izmene
            if (request.Specialization != null)
                profile.Specialization = request.Specialization.Trim();

            if (request.YearsOfExperience.HasValue)
            {
                if (request.YearsOfExperience.Value < 0 || request.YearsOfExperience.Value > 70)
                    throw new ArgumentException("Godine iskustva moraju biti izmedju 0 i 70.");
                profile.YearsOfExperience = request.YearsOfExperience.Value;
            }

            if (request.Description != null)
                profile.Description = request.Description.Trim();

            profile.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateTrainerProfileAsync(profile);

            // Reload sa User podacima za response
            var updated = await _userRepository.GetTrainerProfileByUserIdAsync(userId);
            return MapToProfileResponse(updated!);
        }

        private static TrainerProfileResponse MapToProfileResponse(TrainerProfile profile)
        {
            return new TrainerProfileResponse
            {
                Id = profile.Id,
                UserId = profile.UserId,
                Username = profile.User?.Username ?? string.Empty,
                FirstName = profile.User?.FirstName ?? string.Empty,
                LastName = profile.User?.LastName ?? string.Empty,
                ProfileImageUrl = profile.User?.ProfileImageUrl,
                Specialization = profile.Specialization,
                YearsOfExperience = profile.YearsOfExperience,
                Description = profile.Description,
                UpdatedAt = profile.UpdatedAt
            };

        }

        public async Task<IEnumerable<TrainerSearchResponse>> SearchTrainersAsync(TrainerSearchRequest request)
        {
            var results = await _userRepository.SearchTrainersAsync(
                request.Keyword,
                request.Specialization,
                request.MinRating,
                request.SortBy);

            return results.Select(r => new TrainerSearchResponse
            {
                Id = r.user.Id,
                Username = r.user.Username,
                FirstName = r.user.FirstName,
                LastName = r.user.LastName,
                Location = r.user.Location,
                ProfileImageUrl = r.user.ProfileImageUrl,
                Specialization = r.profile?.Specialization,
                YearsOfExperience = r.profile?.YearsOfExperience,
                Description = r.profile?.Description,
                AverageRating = r.avgRating,
                TotalReviews = r.reviewCount
            });
        }
    }
}

