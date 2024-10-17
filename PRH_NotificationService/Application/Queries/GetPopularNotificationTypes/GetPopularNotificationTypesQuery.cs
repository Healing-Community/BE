using Application.Commons;
using MediatR;

namespace Application.Queries.GetPopularNotificationTypes
{
    public record GetPopularNotificationTypesQuery() : IRequest<BaseResponse<Dictionary<string, int>>>;
}
