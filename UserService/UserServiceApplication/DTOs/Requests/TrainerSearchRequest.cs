using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceApplication.DTOs.Requests
{
    public class TrainerSearchRequest
    {
        public string? Keyword { get; set; }
        public string? Specialization { get; set; }
        public double? MinRating { get; set; }
        public string? SortBy { get; set; }  // "rating", "experience", "name"
    }
}
