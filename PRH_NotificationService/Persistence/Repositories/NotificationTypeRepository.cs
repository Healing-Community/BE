using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class NotificationTypeRepository(HFDBNotificationServiceContext context) : INotificationTypeRepository
    {
        public async Task Create(NotificationType entity)
        {
            await context.NotificationTypes.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var notificationType = await context.NotificationTypes.FirstOrDefaultAsync(x => x.NotificationTypeId == id);
            if (notificationType == null) return;
            context.NotificationTypes.Remove(notificationType);
            await context.SaveChangesAsync();
        }

        public async Task<NotificationType> GetByIdAsync(string id)
        {
            return await context.NotificationTypes.FirstAsync(x => x.NotificationTypeId == id);
        }

        public async Task<NotificationType> GetByPropertyAsync(Expression<Func<NotificationType, bool>> predicate)
        {
            return await context.NotificationTypes.AsNoTracking().FirstOrDefaultAsync(predicate) ?? new NotificationType { NotificationTypeId = Ulid.NewUlid().ToString() };
        }

        public async Task<IEnumerable<NotificationType>> GetsAsync()
        {
            return await context.NotificationTypes.ToListAsync();
        }

        public async Task Update(string id, NotificationType entity)
        {
            var existingNotificationType = await context.NotificationTypes.FirstOrDefaultAsync(x => x.NotificationTypeId == id);
            if (existingNotificationType == null) return;
            context.Entry(existingNotificationType).CurrentValues.SetValues(entity);
            context.Entry(existingNotificationType).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task<NotificationType?> GetByNameAsync(string notificationTypeName)
        {
            return await context.NotificationTypes
                                 .FirstOrDefaultAsync(nt => nt.Name == notificationTypeName);
        }
    }
}
