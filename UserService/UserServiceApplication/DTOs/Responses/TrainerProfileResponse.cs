using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceApplication.DTOs.Responses
{
    public class TrainerProfileResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string? Specialization { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Description { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
