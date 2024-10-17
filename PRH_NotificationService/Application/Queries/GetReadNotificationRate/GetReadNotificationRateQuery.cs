using Application.Commons;
using MediatR;

namespace Application.Queries.GetReadNotificationRate
{
    public record GetReadNotificationRateQuery() : IRequest<BaseResponse<double>>;
}
