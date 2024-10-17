using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly HFDBNotificationServiceContext _context;

        public NotificationRepository(HFDBNotificationServiceContext context)
        {
            _context = context;
        }

        public async Task Create(Notification entity)
        {
            await _context.Notifications.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return;
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<Notification> GetByIdAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            return notification ?? new Notification();
        }

        public async Task<Notification> GetByPropertyAsync(Expression<Func<Notification, bool>> predicate)
        {
            return await _context.Notifications.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new Notification();
        }

        public async Task<IEnumerable<Notification>> GetsAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task Update(Guid id, Notification entity)
        {
            var existingNotification = await _context.Notifications.FindAsync(id);
            if (existingNotification == null) return;
            _context.Entry(existingNotification).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
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
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return;

            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;

            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task ArchiveUnreadNotificationsAsync(Guid userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            _context.Notifications.UpdateRange(unreadNotifications);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> GetUserNotificationPreferenceAsync(Guid userId, Guid notificationTypeId)
        {
            var userNotificationPreference = await _context.UserNotificationPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(unp => unp.UserId == userId && unp.NotificationTypeId == notificationTypeId);

            return userNotificationPreference?.IsSubscribed ?? false;
        }

        public async Task<List<UserNotificationPreference>> GetUserNotificationPreferencesAsync(List<Guid> userIds, Guid notificationTypeId)
        {
            return await _context.UserNotificationPreferences
                .AsNoTracking()
                .Where(unp => userIds.Contains(unp.UserId) && unp.NotificationTypeId == notificationTypeId && unp.IsSubscribed)
                .ToListAsync();
        }

        public async Task<NotificationType?> GetNotificationTypeByEnum(NotificationTypeEnum typeEnum)
        {
            return await _context.NotificationTypes
                .FirstOrDefaultAsync(nt => nt.Name == typeEnum.ToString());
        }

        public async Task CreateNotificationsAsync(IEnumerable<Notification> notifications)
        {
            await _context.Set<Notification>().AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task<double> GetReadNotificationRateAsync()
        {
            var totalNotifications = await _context.Notifications.CountAsync();
            if (totalNotifications == 0) return 0;

            var readNotifications = await _context.Notifications.CountAsync(n => n.IsRead);
            return (double)readNotifications / totalNotifications * 100;
        }

        public async Task<Dictionary<string, int>> GetPopularNotificationTypesAsync()
        {
            return await _context.Notifications
                .GroupBy(n => n.NotificationType.Name)
                .Select(g => new { NotificationType = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToDictionaryAsync(x => x.NotificationType, x => x.Count);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }
    }
}
