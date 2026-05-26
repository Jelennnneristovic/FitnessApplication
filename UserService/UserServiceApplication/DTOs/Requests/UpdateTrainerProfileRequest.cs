using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceApplication.DTOs.Requests
{
    public class UpdateTrainerProfileRequest
    {
        public string? Specialization { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Description { get; set; }
    }
}
