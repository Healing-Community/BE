using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface ITokenRepository : ICreateRepository<Token>, IReadRepository<Token>, IUpdateRepository<Token>,
    IDeleteRepository
{
}