using Application.Commons;
using MediatR;

namespace Application.Queries.GetUnreadNotificationCount
{
    public record GetUnreadNotificationCountQuery(string UserId) : IRequest<BaseResponse<int>>;
}
