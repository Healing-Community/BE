using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class NotificationRepository(HFDBNotificationServiceContext context) : INotificationRepository
    {
        public async Task<List<Notification>> GetNotificationsByUserAsync(Guid userId, bool includeRead)
        {
            var query = context.Notifications
                .Where(n => n.UserId == userId);

            if (!includeRead)
            {
                query = query.Where(n => !n.IsRead);
            }

            return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task Create(Notification entity)
        {
            await context.Notifications.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var notification = await context.Notifications.FindAsync(id);
            if (notification == null) return;
            context.Notifications.Remove(notification);
            await context.SaveChangesAsync();
        }

        public async Task<Notification> GetByIdAsync(Guid id)
        {
            return await context.Notifications.FirstAsync(x => x.NotificationId == id);

        }

        public async Task<Notification> GetByPropertyAsync(Expression<Func<Notification, bool>> predicate)
        {
            return await context.Notifications.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new Notification();
        }

        public async Task<IEnumerable<Notification>> GetsAsync()
        {
            return await context.Notifications.ToListAsync();
        }

        public async Task Update(Guid id, Notification entity)
        {
            var existingNotification = await context.Notifications.FirstOrDefaultAsync(x => x.NotificationId == id);
            if (existingNotification == null) return;
            context.Entry(existingNotification).CurrentValues.SetValues(entity);
            context.Entry(existingNotification).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(Guid userId, Guid notificationTypeId, string message)
        {
            if (!await GetUserNotificationPreferenceAsync(userId, notificationTypeId)) return;

            var createNotification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = userId,
                NotificationTypeId = notificationTypeId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsRead = false,
                Message = message
            };

            await Create(createNotification);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await context.Notifications.FindAsync(notificationId);
            if (notification == null) return;

            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;

            context.Notifications.Update(notification);
            await context.SaveChangesAsync();
        }

        public async Task ArchiveUnreadNotificationsAsync(Guid userId)
        {
            var unreadNotifications = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            context.Notifications.UpdateRange(unreadNotifications);
            await context.SaveChangesAsync();
        }

        public async Task<bool> GetUserNotificationPreferenceAsync(Guid userId, Guid notificationTypeId)
        {
            var userNotificationPreference = await context.UserNotificationPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(unp => unp.UserId == userId && unp.NotificationTypeId == notificationTypeId);

            return userNotificationPreference?.IsSubscribed ?? false;
        }

        public async Task<List<UserNotificationPreference>> GetUserNotificationPreferencesAsync(List<Guid> userIds, Guid notificationTypeId)
        {
            return await context.UserNotificationPreferences
                .AsNoTracking()
                .Where(unp => userIds.Contains(unp.UserId) && unp.NotificationTypeId == notificationTypeId && unp.IsSubscribed)
                .ToListAsync();
        }

        public async Task CreateNotificationsAsync(IEnumerable<Notification> notifications)
        {
            await context.Set<Notification>().AddRangeAsync(notifications);
            await context.SaveChangesAsync();
        }

        public async Task<double> GetReadNotificationRateAsync()
        {
            var totalNotifications = await context.Notifications.CountAsync();
            if (totalNotifications == 0) return 0;

            var readNotifications = await context.Notifications.CountAsync(n => n.IsRead);
            return (double)readNotifications / totalNotifications * 100;
        }

        public async Task<Dictionary<string, int>> GetPopularNotificationTypesAsync()
        {
            return await context.Notifications
                .GroupBy(n => n.NotificationType.Name)
                .Select(g => new { NotificationType = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToDictionaryAsync(x => x.NotificationType, x => x.Count);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }
    }
}
