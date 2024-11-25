using System;
using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository;

public interface IUserReferenceRepository : ICreateRepository<UserPreference>, IReadRepository<UserPreference>
{

}