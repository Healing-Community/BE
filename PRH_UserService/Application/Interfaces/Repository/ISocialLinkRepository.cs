using System;
using System.Linq.Expressions;
using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface ISocialLinkRepository : IReadRepository<SocialLink>, ICreateRepository<SocialLink>, IUpdateRepository<SocialLink>, IDeleteRepository
{
    public Task<IEnumerable<SocialLink>?> GetsByPropertyAsync(Expression<Func<SocialLink, bool>> predicate);
}