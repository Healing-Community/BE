using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.NotifyFollowers
{
    public record NotifyFollowersCommand(NotifyFollowersRequestDto NotifyFollowersRequestDto) : IRequest<BaseResponse<string>>;
}
