using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetTransactionHistory
{
    public record GetTransactionHistoryQuery(string UserId) : IRequest<BaseResponse<IEnumerable<Payment>>>;
}
