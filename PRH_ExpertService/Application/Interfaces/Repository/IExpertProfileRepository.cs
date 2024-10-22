using Application.Interfaces.GenericRepository;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces.Repository
{
    public interface IExpertProfileRepository : IReadRepository<ExpertProfile>, ICreateRepository<ExpertProfile>, IUpdateRepository<ExpertProfile>, IDeleteRepository
    {
    }
}
