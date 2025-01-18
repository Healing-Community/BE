using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface INotificationRepository : IReadRepository<Notification>, ICreateRepository<Notification>, IUpdateRepository<Notification>, IDeleteRepository
    {
        Task CreateNotificationAsync(string userId, string notificationTypeId, string message);
        Task MarkAsReadAsync(string notificationId);
        Task ArchiveUnreadNotificationsAsync(string userId);
        Task<bool> GetUserNotificationPreferenceAsync(string userId, string notificationTypeId);
        Task<List<UserNotificationPreference>> GetUserNotificationPreferencesAsync(List<string> userIds, string notificationTypeId);
        Task CreateNotificationsAsync(IEnumerable<Notification> notifications);
        Task <double> GetReadNotificationRateAsync();
        Task<Dictionary<string, int>> GetPopularNotificationTypesAsync();
        Task<int> GetUnreadCountAsync(string userId);
        Task<List<Notification>> GetNotificationsByUserAsync(string userId);
    }
}
