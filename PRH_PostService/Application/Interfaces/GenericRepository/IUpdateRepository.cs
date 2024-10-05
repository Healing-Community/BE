using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.GenericRepository
{
    public interface IUpdateRepository<in T> where T : class
    {
        Task Update(Guid id, T entity);
    }
}
