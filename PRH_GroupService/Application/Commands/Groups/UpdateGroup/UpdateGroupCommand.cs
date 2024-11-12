using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Groups.UpdateGroup
{
    public record UpdateGroupCommand(string Id, GroupDto GroupDto) : IRequest<BaseResponse<string>>
    {
    }
}
