using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserServiceDomain.Enums;

namespace UserServiceApplication.DTOs.Requests
{
    public class UpdateUserRequest
    {
        public string? FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        public UserGender? Gender { get; set; }

        public string? Location { get; set; } = string.Empty;
    }
}