using NotificationServiceApplication.DTOs.Requests;
using NotificationServiceApplication.DTOs.Responses;
using NotificationServiceApplication.Interfaces;
using NotificationServiceDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServiceApplication.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<NotificationResponse> CreateAsync(CreateNotificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Naslov je obavezan.");

            if (string.IsNullOrWhiteSpace(request.Content))
                throw new ArgumentException("Sadrzaj je obavezan.");

            if (request.UserId == Guid.Empty)
                throw new ArgumentException("UserId je obavezan.");

            var notification = new Notification
            {
                UserId = request.UserId,
                Title = request.Title.Trim(),
                Content = request.Content.Trim(),
                Type = request.Type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ReadAt = null
            };

            await _repository.AddAsync(notification);
            return MapToResponse(notification);
        }

        public async Task<IEnumerable<NotificationResponse>> GetMyNotificationsAsync(Guid userId, bool unreadOnly)
        {
            var notifications = await _repository.GetByUserIdAsync(userId, unreadOnly);
            return notifications.Select(MapToResponse);
        }

        public async Task<NotificationResponse> MarkAsReadAsync(string id, Guid userId)
        {
            var notification = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Notifikacija nije pronadjena.");

            // Provera vlasnistva - korisnik moze da menja samo svoje notifikacije
            if (notification.UserId != userId)
                throw new UnauthorizedAccessException("Nemate pravo da menjate ovu notifikaciju.");

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _repository.UpdateAsync(notification);
            }

            return MapToResponse(notification);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _repository.MarkAllAsReadAsync(userId);
        }

        private static NotificationResponse MapToResponse(Notification n)
        {
            return new NotificationResponse
            {
                Id = n.Id ?? string.Empty,
                UserId = n.UserId,
                Title = n.Title,
                Content = n.Content,
                IsRead = n.IsRead,
                Type = n.Type,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            };
        }
    }
}
