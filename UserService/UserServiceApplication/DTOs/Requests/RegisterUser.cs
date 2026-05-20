using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UserServiceDomain.Enums;

namespace UserServiceApplication.DTOs.Requests
{
    public class RegisterUser
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public UserGender Gender { get; set; }
        public string Location { get; set; } = string.Empty;
        public UserRole Role { get; set; }  // Trainer ili Client
    }
}
