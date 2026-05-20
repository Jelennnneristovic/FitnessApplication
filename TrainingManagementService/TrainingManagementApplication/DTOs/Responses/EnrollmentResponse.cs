using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Enums;

namespace TrainingManagementApplication.DTOs.Responses
{
    public class EnrollmentResponse
    {
        public Guid Id { get; set; }
        public Guid TrainingPlanId { get; set; }
        public string TrainingPlanTitle { get; set; } = string.Empty;
        public Guid TrainerId { get; set; }
        public Guid ClientId { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string? RejectionReason { get; set; }
        public string? ClientNote { get; set; }
    }
}
