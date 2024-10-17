using Application.Commons;
using MediatR;

namespace Application.Commands.CreateNotificationType
{
    public record CreateNotificationTypeCommand(string Name, string Description) : IRequest<BaseResponse<string>>;
}
