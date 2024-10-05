using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.GenericRepository
{
    public interface IReadRepository<T>
    {
        Task<IEnumerable<T>> GetsAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByPropertyAsync(Expression<Func<T, bool>> predicate);
    }
}
