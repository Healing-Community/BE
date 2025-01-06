using MediatR;
using Application.Commons.DTOs;
using Application.Commons;

namespace Application.Queries.GetRecentRatings
{
    public class GetRecentRatingsQuery : IRequest<BaseResponse<IEnumerable<RecentRatingDTO>>>
    {
    }
}
