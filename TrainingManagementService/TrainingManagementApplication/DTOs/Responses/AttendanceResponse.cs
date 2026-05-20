using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingManagementApplication.DTOs.Responses
{
    public class AttendanceResponse
    {
        public Guid Id { get; set; }
        public Guid TrainingSessionId { get; set; }
        public DateTime SessionStartTime { get; set; }
        public string TrainingPlanTitle { get; set; } = string.Empty;
        public Guid ClientId { get; set; }
        public bool Attended { get; set; }
        public DateTime MarkedAt { get; set; }
        public Guid MarkedByUserId { get; set; }
        public string? Notes { get; set; }
    }
}
