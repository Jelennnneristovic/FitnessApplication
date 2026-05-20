using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Enums;

namespace TrainingManagementApplication.DTOs.Responses
{
    public class TrainingPlanResponse
    {
        public Guid Id { get; set; }
        public Guid TrainerId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TrainingType Type { get; set; }
        public decimal Price { get; set; }
        public int MaxParticipants { get; set; }
        public int DurationMinutes { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public TrainingPlanStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
