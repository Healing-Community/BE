using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HFDBNotificationServiceContext _context;

        public UserRepository(HFDBNotificationServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetFollowersAsync(Guid userId)
        {
            return await _context.Users
                .Where(u => _context.UserFollows
                .Any(f => f.FolloweeId == userId && f.FollowerId == u.UserId))
                .ToListAsync();
        }
    }
}

