using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Enums;

namespace TrainingManagementDomain.Entities
{
    public class Enrollment
    {
        public Guid Id { get; set; }
        public Guid TrainingPlanId { get; set; }
        public Guid ClientId { get; set; }          // FK ka korisniku iz UserService-a
        public EnrollmentStatus Status { get; set; } // Pending, Approved, Rejected, Cancelled
        public DateTime RequestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string? RejectionReason { get; set; }
        public string? ClientNote { get; set; }

        public TrainingPlan TrainingPlan { get; set; } = null!;
    }
}
