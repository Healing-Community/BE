using System;

namespace Application.Interfaces.GenericRepository;

public interface IGenericRepository<T> : IReadRepository<T>, ICreateRepository<T>, IUpdateRepository<T>, IDeleteRepository where T : class
{

}
