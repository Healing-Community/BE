using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface IUserNotificationPreferenceRepository
    {
        Task<UserNotificationPreference?> GetByUserIdAndNotificationTypeIdAsync(Guid userId, Guid notificationTypeId);
        Task Create(UserNotificationPreference preference);
        Task Update(UserNotificationPreference preference);
    }
}
