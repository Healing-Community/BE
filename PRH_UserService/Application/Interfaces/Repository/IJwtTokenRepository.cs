using Application.Interfaces.GenericRepository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface IJwtTokenRepository
    {
        string GenerateToken(User user);
        string GenerateVerificationToken(User user);
        bool ValidateToken(string token, out Guid userId);
    }
}
