using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface INotificationRepository : IReadRepository<Notification>, ICreateRepository<Notification>, IUpdateRepository<Notification>, IDeleteRepository
    {
        Task CreateNotificationAsync(Guid userId, Guid notificationTypeId, string message);
        Task MarkAsReadAsync(Guid notificationId);
        Task ArchiveUnreadNotificationsAsync(Guid userId);
        Task<bool> GetUserNotificationPreferenceAsync(Guid userId, Guid notificationTypeId);
        Task<List<UserNotificationPreference>> GetUserNotificationPreferencesAsync(List<Guid> userIds, Guid notificationTypeId);
        Task CreateNotificationsAsync(IEnumerable<Notification> notifications);
        Task<double> GetReadNotificationRateAsync();
        Task<Dictionary<string, int>> GetPopularNotificationTypesAsync();
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<List<Notification>> GetNotificationsByUserAsync(Guid userId, bool includeRead);
    }
}
