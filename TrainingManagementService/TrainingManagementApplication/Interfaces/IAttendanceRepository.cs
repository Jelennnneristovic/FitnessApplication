using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingManagementDomain.Entities;

namespace TrainingManagementApplication.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<Attendance?> GetByIdAsync(Guid id);
        Task<Attendance?> GetBySessionAndClientAsync(Guid sessionId, Guid clientId);
        Task<IEnumerable<Attendance>> GetByClientIdAsync(Guid clientId);
        Task<IEnumerable<Attendance>> GetBySessionIdAsync(Guid sessionId);
        Task AddAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
    }
}
