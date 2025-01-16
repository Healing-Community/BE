using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Queries.GetTotalRevenueAdmin
{
    public class GetTotalRevenueAdminQuery : IRequest<BaseResponse<decimal>>
    {
    }
}
