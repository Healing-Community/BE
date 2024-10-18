using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface INotificationTypeRepository : IReadRepository<NotificationType>, ICreateRepository<NotificationType>, IUpdateRepository<NotificationType>, IDeleteRepository
    {
        Task<NotificationType?> GetByNameAsync(string notificationTypeName);

    }
}
