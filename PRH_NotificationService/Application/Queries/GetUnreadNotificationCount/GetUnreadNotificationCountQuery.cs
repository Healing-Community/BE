using Application.Commons;
using MediatR;

namespace Application.Queries.GetUnreadNotificationCount
{
    public record GetUnreadNotificationCountQuery(Guid UserId) : IRequest<BaseResponse<int>>;
}
