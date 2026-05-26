using NotificationServiceDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServiceApplication.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(string id);
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, bool unreadOnly);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
