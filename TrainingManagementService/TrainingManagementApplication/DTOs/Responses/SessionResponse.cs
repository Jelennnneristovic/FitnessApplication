using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Enums;

namespace TrainingManagementApplication.DTOs.Responses
{
    public class SessionResponse
    {
        public Guid Id { get; set; }
        public Guid TrainingPlanId { get; set; }
        public string TrainingPlanTitle { get; set; } = string.Empty;
        public Guid TrainerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TrainingSessionStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
