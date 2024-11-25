using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IExpertAvailabilityRepository : IReadRepository<ExpertAvailability>, ICreateRepository<ExpertAvailability>, IUpdateRepository<ExpertAvailability>, IDeleteRepository
    {
        Task<ExpertAvailability?> GetByDateAndTimeAsync(string expertProfileId, DateOnly availableDate, TimeOnly startTime, TimeOnly endTime);
        Task<IEnumerable<ExpertAvailability>> GetByExpertProfileIdAsync(string expertProfileId);
        Task<ExpertAvailability?> GetOverlappingAvailabilityAsync(string expertProfileId, DateOnly availableDate, TimeOnly startTime, TimeOnly endTime);
    }
}
