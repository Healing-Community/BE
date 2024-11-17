using Application.Commons;
using MediatR;

namespace Application.Commands.Groups.DeleteGroup
{
    public record DeleteGroupCommand(string Id) : IRequest<BaseResponse<string>>
    {
    }
}
