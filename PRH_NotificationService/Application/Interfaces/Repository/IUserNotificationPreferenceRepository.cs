using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IUserNotificationPreferenceRepository
    {
        Task<UserNotificationPreference?> GetByUserIdAndNotificationTypeIdAsync(string userId, string notificationTypeId);
        Task Create(UserNotificationPreference preference);
        Task Update(UserNotificationPreference preference);
    }
}
