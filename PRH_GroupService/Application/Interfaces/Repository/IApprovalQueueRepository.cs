using Application.Interfaces.GenericRepository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface IApprovalQueueRepository : IReadRepository<ApprovalQueue>, ICreateRepository<ApprovalQueue>, IUpdateRepository<ApprovalQueue>, IDeleteRepository
    {
        Task<IEnumerable<ApprovalQueue>> GetPendingApprovalsByGroupIdAsync(string groupId);
    }

}
