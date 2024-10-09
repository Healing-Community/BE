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
    public class NotificationTypeRepository : INotificationTypeRepository
    {
        private readonly HFDBNotificationServiceContext _context;

        public NotificationTypeRepository(HFDBNotificationServiceContext context)
        {
            _context = context;
        }

        public async Task<NotificationType?> GetByNameAsync(string name)
        {
            return await _context.NotificationTypes.FirstOrDefaultAsync(nt => nt.Name == name);
        }
    }
}
