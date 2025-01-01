using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPlatformFeeRepository : ICreateRepository<PlatformFee>, IReadRepository<PlatformFee>, IUpdateRepository<PlatformFee>, IDeleteRepository
{

}
