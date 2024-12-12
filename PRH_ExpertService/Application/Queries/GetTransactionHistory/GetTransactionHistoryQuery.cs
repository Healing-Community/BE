using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.GetTransactionHistory
{
    public record GetTransactionHistoryQuery() : IRequest<BaseResponse<ICollection<TransactionHistoryDTO>>>;
}
