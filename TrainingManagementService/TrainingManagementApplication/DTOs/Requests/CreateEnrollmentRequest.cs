using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingManagementApplication.DTOs.Requests
{
    public class CreateEnrollmentRequest
    {
        public Guid TrainingPlanId { get; set; }
        public string? ClientNote { get; set; }
    }
}
