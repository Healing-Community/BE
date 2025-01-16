using System.Linq.Expressions;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;

namespace Persistence.Repositories
{
    public class NotificationRepository(HFDBNotificationServiceContext context) : INotificationRepository
    {
        public async Task<List<Notification>> GetNotificationsByUserAsync(string userId)
        {
            var query = context.Notifications
                .Where(n => n.UserId == userId);

            return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task Create(Notification entity)
        {
            await context.Notifications.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var notification = await context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == id);
            if (notification == null) return;
            context.Notifications.Remove(notification);
            await context.SaveChangesAsync();
        }

        public async Task<Notification> GetByIdAsync(string id)
        {
            return await context.Notifications.FirstAsync(x => x.NotificationId == id);
        }

        public async Task<Notification> GetByPropertyAsync(Expression<Func<Notification, bool>> predicate)
        {
            return await context.Notifications.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new Notification { NotificationId = Ulid.NewUlid().ToString(), UserId = Ulid.NewUlid().ToString(), NotificationTypeId = Ulid.NewUlid().ToString() };
        }

        public async Task<IEnumerable<Notification>> GetsAsync()
        {
            return await context.Notifications.ToListAsync();
        }

        public async Task Update(string id, Notification entity)
        {
            var existingNotification = await context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == id);
            if (existingNotification == null) return;
            context.Entry(existingNotification).CurrentValues.SetValues(entity);
            context.Entry(existingNotification).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(string userId, string notificationTypeId, string message)
        {
            if (!await GetUserNotificationPreferenceAsync(userId, notificationTypeId)) return;

            var createNotification = new Notification
            {
                NotificationId = Ulid.NewUlid().ToString(),
                UserId = userId,
                NotificationTypeId = notificationTypeId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsRead = false,
                Message = message
            };

            await Create(createNotification);
        }

        public async Task MarkAsReadAsync(string notificationId)
        {
            var notification = await context.Notifications.FindAsync(notificationId);
            if (notification == null) return;

            notification.IsRead = true;
            await context.SaveChangesAsync();
        }

        public async Task ArchiveUnreadNotificationsAsync(string userId)
        {
            var unreadNotifications = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await context.SaveChangesAsync();
        }

        public async Task<bool> GetUserNotificationPreferenceAsync(string userId, string notificationTypeId)
        {
            var preference = await context.UserNotificationPreferences
                .FirstOrDefaultAsync(unp => unp.UserId == userId && unp.NotificationTypeId == notificationTypeId);

            return preference?.IsSubscribed ?? false;
        }

        public async Task<List<UserNotificationPreference>> GetUserNotificationPreferencesAsync(List<string> userIds, string notificationTypeId)
        {
            return await context.UserNotificationPreferences
                .Where(unp => userIds.Contains(unp.UserId) && unp.NotificationTypeId == notificationTypeId && unp.IsSubscribed)
                .ToListAsync();
        }

        public async Task CreateNotificationsAsync(IEnumerable<Notification> notifications)
        {
            await context.Notifications.AddRangeAsync(notifications);
            await context.SaveChangesAsync();
        }

        public async Task<double> GetReadNotificationRateAsync()
        {
            var totalNotifications = await context.Notifications.CountAsync();
            var readNotifications = await context.Notifications.CountAsync(n => n.IsRead);

            return totalNotifications == 0 ? 0 : (double)readNotifications / totalNotifications;
        }

        public async Task<Dictionary<string, int>> GetPopularNotificationTypesAsync()
        {
            return await context.Notifications
                .GroupBy(n => n.NotificationTypeId)
                .OrderByDescending(g => g.Count())
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
        }
    }
}
