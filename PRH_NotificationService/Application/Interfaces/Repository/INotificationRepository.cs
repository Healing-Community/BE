using Application.Interfaces.GenericRepository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface INotificationRepository : IReadRepository<Notification>, ICreateRepository<Notification>, IUpdateRepository<Notification>, IDeleteRepository
    {
        Task CreateNotificationAsync(Guid userId, Guid notificationTypeId, string message);
        Task MarkAsReadAsync(Guid notificationId);
        Task ArchiveUnreadNotificationsAsync(Guid userId);
    }
}
