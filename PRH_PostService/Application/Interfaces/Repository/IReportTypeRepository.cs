using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IReportTypeRepository : IReadRepository<ReportType>, ICreateRepository<ReportType>, IUpdateRepository<ReportType>, IDeleteRepository
    {
    }
}
