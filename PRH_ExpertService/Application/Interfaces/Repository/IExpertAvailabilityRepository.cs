using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IExpertAvailabilityRepository : IReadRepository<ExpertAvailability>, ICreateRepository<ExpertAvailability>, IUpdateRepository<ExpertAvailability>, IDeleteRepository
    {
        Task<ExpertAvailability?> GetByDateAndTimeAsync(string expertProfileId, DateTime availableDate, TimeSpan startTime, TimeSpan endTime);
        Task<IEnumerable<ExpertAvailability>> GetByExpertProfileIdAsync(string expertProfileId);
    }
}
