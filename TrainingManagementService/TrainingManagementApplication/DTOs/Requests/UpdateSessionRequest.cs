using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Enums;

namespace TrainingManagementApplication.DTOs.Requests
{
    public class UpdateSessionRequest
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TrainingSessionStatus? Status { get; set; }
        public string? Notes { get; set; }
    }
}
