using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class NotificationTypeRepository : INotificationTypeRepository
    {
        private readonly HFDBNotificationServiceContext _context;

        public NotificationTypeRepository(HFDBNotificationServiceContext context)
        {
            _context = context;
        }

        public async Task Create(NotificationType entity)
        {
            await _context.NotificationTypes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var notificationType = await _context.NotificationTypes.FirstOrDefaultAsync(x => x.NotificationTypeId == id);
            if (notificationType == null) return;
            _context.NotificationTypes.Remove(notificationType);
            await _context.SaveChangesAsync();
        }

        public async Task<NotificationType> GetByIdAsync(Guid id)
        {
            return await _context.NotificationTypes.FirstAsync(x => x.NotificationTypeId == id);
        }

        public async Task<NotificationType> GetByPropertyAsync(Expression<Func<NotificationType, bool>> predicate)
        {
            return await _context.NotificationTypes.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new NotificationType();
        }

        public async Task<IEnumerable<NotificationType>> GetsAsync()
        {
            return await _context.NotificationTypes.ToListAsync();
        }

        public async Task Update(Guid id, NotificationType entity)
        {
            var existingNotificationType = await _context.NotificationTypes.FirstOrDefaultAsync(x => x.NotificationTypeId == id);
            if (existingNotificationType == null) return;
            _context.Entry(existingNotificationType).CurrentValues.SetValues(entity);
            _context.Entry(existingNotificationType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<NotificationType?> GetByNameAsync(string notificationTypeName)
        {
            return await _context.NotificationTypes
                                 .FirstOrDefaultAsync(nt => nt.Name == notificationTypeName);
        }
    }
}
