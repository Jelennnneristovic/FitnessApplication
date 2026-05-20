using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceDomain.Enums;

namespace UserServiceApplication.DTOs.Responses
{
    public class UserDTO
    {

        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
      
        public UserStatus Status { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
