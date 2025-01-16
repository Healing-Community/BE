using MediatR;
using Application.Commons;
using Domain.Entities;

namespace Application.Queries.GetNotificationTypes
{
    public class GetNotificationTypesQuery : IRequest<BaseResponse<List<NotificationType>>>
    {
    }
}
