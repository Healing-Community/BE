using Application.Interfaces.GenericRepository;

public interface IMongoRepository<T> : IReadRepository<T>, ICreateRepository<T>, IUpdateRepository<T>, IDeleteRepository
    where T : class
{

}