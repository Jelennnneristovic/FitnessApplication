using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Enums;

namespace TrainingManagementDomain.Entities
{
    public class TrainingSession
    {
        public Guid Id { get; set; }
        public Guid TrainingPlanId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TrainingSessionStatus Status { get; set; }  // Scheduled, Completed, Cancelled
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public TrainingPlan TrainingPlan { get; set; } = null!;
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
