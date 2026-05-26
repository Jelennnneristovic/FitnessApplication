using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceDomain.Entities
{
    public class TrainerProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Specialization { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Description { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }
}
