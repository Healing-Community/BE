using Application.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface IApprovalQueueRepository : IReadRepository<ApprovalQueue>, ICreateRepository<ApprovalQueue>, IUpdateRepository<ApprovalQueue>, IDeleteRepository
    {
        Task<IEnumerable<ApprovalQueue>> GetPendingApprovalsByGroupIdAsync(string groupId);
        Task<IEnumerable<ApprovalQueue>> GetAllByGroupIdAsync(string groupId);
    }

}
