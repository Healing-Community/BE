using System.Linq.Expressions;

namespace Application.Interfaces.GenericRepository;

public interface IReadRepository<T>
{
    Task<IEnumerable<T>?> GetsAsync();
    Task<T?> GetByIdAsync(string id);
    Task<T?> GetByPropertyAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>?> GetsByPropertyAsync(Expression<Func<T, bool>> predicate);
}