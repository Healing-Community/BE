using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IWorkExperienceRepository : IReadRepository<WorkExperience>, ICreateRepository<WorkExperience>, IUpdateRepository<WorkExperience>, IDeleteRepository
    {
        Task<IEnumerable<WorkExperience>> GetWorkExperiencesByExpertIdAsync(string expertProfileId);
    }
}
