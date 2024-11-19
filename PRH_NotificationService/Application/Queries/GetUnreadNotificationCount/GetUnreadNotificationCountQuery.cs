using Application.Commons;
using MediatR;

namespace Application.Queries.GetUnreadNotificationCount
{
    public record GetUnreadNotificationCountQuery() : IRequest<BaseResponse<int>>;
}
