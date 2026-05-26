using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceApplication.DTOs.Responses
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid TrainerId { get; set; }
        public Guid ClientId { get; set; }
        public string ClientUsername { get; set; } = string.Empty;
        public string ClientFirstName { get; set; } = string.Empty;
        public string ClientLastName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
