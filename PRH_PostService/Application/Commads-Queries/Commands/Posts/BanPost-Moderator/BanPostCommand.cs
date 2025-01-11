using Application.Commons;
using MediatR;
namespace Application.Commads_Queries.Commands.Posts.BanPost_Moderator;

public record BanPostCommand(string PostId, bool IsApprove) : IRequest<BaseResponse<string>>;