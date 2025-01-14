using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Queries.GetTotalRevenueExpert
{
    public class GetTotalRevenueExpertQuery : IRequest<BaseResponse<decimal>>
    {
    }
}
