using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingManagementDomain.Entities
{
    public class Attendance
    {
        public Guid Id { get; set; }
        public Guid TrainingSessionId { get; set; }
        public Guid ClientId { get; set; }
        public bool Attended { get; set; }          // da li je došao
        public DateTime MarkedAt { get; set; }
        public Guid MarkedByUserId { get; set; }    // ko je označio (klijent ili trener)
        public string? Notes { get; set; }

        public TrainingSession TrainingSession { get; set; } = null!;
    }
}
