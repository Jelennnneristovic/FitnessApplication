using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceDomain.Entities
{
    public class TrainerReview
    {
        public Guid Id { get; set; }
        public Guid TrainerId { get; set; }
        public Guid ClientId { get; set; }
        public int Rating { get; set; }          // 1-5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User Trainer { get; set; } = null!;
        public User Client { get; set; } = null!;
    }
}
