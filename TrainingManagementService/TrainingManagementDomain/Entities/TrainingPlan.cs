using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Enums;

namespace TrainingManagementDomain.Entities
{
    public class TrainingPlan
    {
        public Guid Id { get; set; }
        public Guid TrainerId { get; set; }        // FK ka korisniku (treneru) iz UserService-a
        public Guid CategoryId { get; set; }       // FK ka kategoriji
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TrainingType Type { get; set; }     // Individual ili Group
        public decimal Price { get; set; }
        public int MaxParticipants { get; set; }   // 1 za individual, više za grupne
        public int DurationMinutes { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public TrainingPlanStatus Status { get; set; } = TrainingPlanStatus.Active;// Active, Archived
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<TrainingSession> Sessions { get; set; } = new List<TrainingSession>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public Category Category { get; set; } = null!;
    }
}
