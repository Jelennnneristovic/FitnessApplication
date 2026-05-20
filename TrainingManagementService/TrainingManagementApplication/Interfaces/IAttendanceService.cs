using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementApplication.DTOs.Requests;
using TrainingManagementApplication.DTOs.Responses;

namespace TrainingManagementApplication.Interfaces
{
    public interface IAttendanceService
    {
        // Klijent oznacava sebe ili trener oznacava klijenta
        Task<AttendanceResponse> MarkAsync(Guid sessionId, Guid clientId, Guid markedByUserId, MarkAttendanceRequest request);

        // Klijent vidi svoju istoriju
        Task<IEnumerable<AttendanceResponse>> GetMyHistoryAsync(Guid clientId);

        // Trener vidi attendance liste za sesiju
        Task<IEnumerable<AttendanceResponse>> GetBySessionAsync(Guid sessionId, Guid trainerId);
    }
}
