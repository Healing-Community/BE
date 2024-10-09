using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id);
            if (notification == null) return;
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<Notification> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FirstAsync(n => n.NotificationId == id);
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
            var existingNotification = await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id);
            if (existingNotification == null) return;
            _context.Entry(existingNotification).CurrentValues.SetValues(entity);
            _context.Entry(existingNotification).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(Guid userId, Guid notificationTypeId, string message)
        {
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = userId,
                NotificationTypeId = notificationTypeId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsRead = false,
                Message = message
            };

            await Create(notification);
        }
    }
}
