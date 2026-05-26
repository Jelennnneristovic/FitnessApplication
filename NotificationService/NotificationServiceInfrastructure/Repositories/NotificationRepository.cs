using MongoDB.Driver;
using NotificationServiceApplication.Interfaces;
using NotificationServiceDomain.Entities;
using NotificationServiceInfrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationServiceInfrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IMongoCollection<Notification> _notifications;

        public NotificationRepository(MongoDbContext context)
        {
            _notifications = context.Notifications;
        }

        public async Task<Notification?> GetByIdAsync(string id)
        {
            return await _notifications
                .Find(n => n.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, bool unreadOnly)
        {
            var filterBuilder = Builders<Notification>.Filter;
            var filter = filterBuilder.Eq(n => n.UserId, userId);

            if (unreadOnly)
                filter &= filterBuilder.Eq(n => n.IsRead, false);

            return await _notifications
                .Find(filter)
                .SortByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Notification notification)
        {
            await _notifications.InsertOneAsync(notification);
        }

        public async Task UpdateAsync(Notification notification)
        {
            await _notifications.ReplaceOneAsync(n => n.Id == notification.Id, notification);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var filter = Builders<Notification>.Filter.And(
                Builders<Notification>.Filter.Eq(n => n.UserId, userId),
                Builders<Notification>.Filter.Eq(n => n.IsRead, false));

            var update = Builders<Notification>.Update
                .Set(n => n.IsRead, true)
                .Set(n => n.ReadAt, DateTime.UtcNow);

            await _notifications.UpdateManyAsync(filter, update);
        }
    }
}
