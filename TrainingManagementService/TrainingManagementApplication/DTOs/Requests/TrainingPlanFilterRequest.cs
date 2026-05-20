using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Enums;

namespace TrainingManagementApplication.DTOs.Requests
{
    public class TrainingPlanFilterRequest
    {
        public string? Keyword { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? TrainerId { get; set; }
        public TrainingType? Type { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public TrainingPlanStatus? Status { get; set; }
    }
}
