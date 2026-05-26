using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceApplication.DTOs.Responses
{
    public class TrainerRatingResponse
    {
        public Guid TrainerId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
