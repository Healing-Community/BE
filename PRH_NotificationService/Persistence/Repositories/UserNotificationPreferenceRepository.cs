using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UserNotificationPreferenceRepository : IUserNotificationPreferenceRepository
    {
        private readonly HFDBNotificationServiceContext _context;

        public UserNotificationPreferenceRepository(HFDBNotificationServiceContext context)
        {
            _context = context;
        }

        public async Task Create(UserNotificationPreference preference)
        {
            await _context.UserNotificationPreferences.AddAsync(preference);
            await _context.SaveChangesAsync();
        }

        public async Task<UserNotificationPreference?> GetByUserIdAndNotificationTypeIdAsync(Guid userId, Guid notificationTypeId)
        {
            return await _context.UserNotificationPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId && p.NotificationTypeId == notificationTypeId);
        }

        public async Task Update(UserNotificationPreference preference)
        {
            _context.UserNotificationPreferences.Update(preference);
            await _context.SaveChangesAsync();
        }
    }
}
