using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Queries.Reactions.GetShareReactionCount
{
    public record GetShareReactionCountQuery(ShareIdOnlyDto ShareId) : IRequest<BaseResponse<PostReactionCountDto>>;
}
