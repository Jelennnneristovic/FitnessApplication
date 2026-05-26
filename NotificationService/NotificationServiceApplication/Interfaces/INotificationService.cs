using NotificationServiceApplication.DTOs.Requests;
using NotificationServiceApplication.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServiceApplication.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponse> CreateAsync(CreateNotificationRequest request);
        Task<IEnumerable<NotificationResponse>> GetMyNotificationsAsync(Guid userId, bool unreadOnly);
        Task<NotificationResponse> MarkAsReadAsync(string id, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
