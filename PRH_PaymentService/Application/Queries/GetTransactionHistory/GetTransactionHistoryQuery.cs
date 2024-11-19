using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetTransactionHistory
{
    public record GetTransactionHistoryQuery() : IRequest<BaseResponse<IEnumerable<Payment>>>;
}
