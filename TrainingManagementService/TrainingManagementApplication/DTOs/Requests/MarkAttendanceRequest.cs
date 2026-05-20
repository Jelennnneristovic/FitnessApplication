using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingManagementApplication.DTOs.Requests
{
    public class MarkAttendanceRequest
    {
        public bool Attended { get; set; }
        public string? Notes { get; set; }
    }
}
