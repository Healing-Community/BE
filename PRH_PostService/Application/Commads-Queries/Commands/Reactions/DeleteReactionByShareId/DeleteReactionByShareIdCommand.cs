using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Commands.Reactions.DeleteReactionByShareId
{
    public record DeleteReactionByShareIdCommand(ShareIdOnlyDto RemoveReactionDto) : IRequest<BaseResponse<string>>;
}
