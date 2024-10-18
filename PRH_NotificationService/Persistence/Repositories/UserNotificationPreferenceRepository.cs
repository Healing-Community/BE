using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class UserNotificationPreferenceRepository(HFDBNotificationServiceContext context) : IUserNotificationPreferenceRepository
    {
        public async Task Create(UserNotificationPreference preference)
        {
            await context.UserNotificationPreferences.AddAsync(preference);
            await context.SaveChangesAsync();
        }

        public async Task<UserNotificationPreference?> GetByUserIdAndNotificationTypeIdAsync(Guid userId, Guid notificationTypeId)
        {
            return await context.UserNotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId && p.NotificationTypeId == notificationTypeId);
        }

        public async Task Update(UserNotificationPreference preference)
        {
            context.UserNotificationPreferences.Update(preference);
            await context.SaveChangesAsync();
        }
    }
}
