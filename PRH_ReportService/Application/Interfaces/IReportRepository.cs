using Application.Interfaces.GenericRepository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
   public interface IReportRepository : IReadRepository<Report>, ICreateRepository<Report>, IDeleteRepository, IUpdateRepository<Report>
    {

    }
}
