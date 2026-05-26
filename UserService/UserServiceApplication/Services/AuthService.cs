using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceApplication.DTOs;
using UserServiceApplication.DTOs.Requests;
using UserServiceApplication.DTOs.Responses;
using UserServiceApplication.Interfaces;
using UserServiceDomain.Entities;
using UserServiceDomain.Enums;
using DomainUser = UserServiceDomain.Entities.User;
using UserResponse = UserServiceApplication.DTOs.Responses.UserDTO;


namespace UserServiceApplication.Services
{
    public class AuthService :IAuthService 
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
        public async Task<Auth> RegisterAsync(RegisterUser request)
        {
            if (request.Role == UserRole.Admin)
                throw new InvalidOperationException("Admin se ne moze registrovati kroz sistem.");

            if (await _userRepository.GetByUsernameAsync(request.Username) is not null)
                throw new InvalidOperationException("Username vec postoji.");

            if (await _userRepository.GetByEmailAsync(request.Email) is not null)
                throw new InvalidOperationException("Email vec postoji.");

            var hashedPasswords = new PasswordHasher<User>().HashPassword(new User(), request.Password);


            var status = request.Role == UserRole.Trainer
                ? UserStatus.PendingApproval
                : UserStatus.Active;

            var user = new DomainUser
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = hashedPasswords,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Location = request.Location,
                Status = status,
                Role = request.Role,
                RegistrationDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            // Ako je trener, kreiraj prazan profil koji ce kasnije popuniti
            if (user.Role == UserRole.Trainer)
            {
                var profile = new TrainerProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Specialization = null,
                    YearsOfExperience = null,
                    Description = null,
                    UpdatedAt = null
                };
                await _userRepository.AddTrainerProfileAsync(profile);
            }

            var token = _tokenService.GenerateToken(user);
            return new Auth
            {
                Token = token,
                User = MapToResponse(user)
            };
        }

        public async Task<Auth> LoginAsync(LoginUser request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username)
                ?? throw new UnauthorizedAccessException("Pogresan username ili lozinka.");

            var hasher = new PasswordHasher<User>();
            var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Pogresan username ili lozinka.");

            if (user.Status == UserStatus.InActive)
                throw new UnauthorizedAccessException("Nalog je deaktiviran.");

            if (user.Status == UserStatus.PendingApproval)
                throw new UnauthorizedAccessException("Nalog ceka odobrenje admina.");

            var token = _tokenService.GenerateToken(user);
            return new Auth
            {
                Token = token,
                User = MapToResponse(user)
            };
        }

        private static UserResponse MapToResponse(DomainUser u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Role = u.Role,
            Status = u.Status,

            
        };
    }
}
