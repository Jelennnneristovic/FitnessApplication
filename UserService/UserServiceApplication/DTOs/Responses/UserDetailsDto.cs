using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserServiceDomain.Enums;

namespace UserServiceApplication.DTOs.Responses
{
    public class UserDetailsDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        public UserGender Gender { get; set; }

        public UserRole Role { get; set; }

        public string Location { get; set; } = string.Empty;

        public UserStatus Status { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
