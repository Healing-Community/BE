using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IRoleRepository : IReadRepository<Role>, ICreateRepository<Role>, IUpdateRepository<Role>,
    IDeleteRepository
{}