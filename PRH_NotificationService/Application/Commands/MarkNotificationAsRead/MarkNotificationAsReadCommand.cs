using Application.Commons;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.MarkNotificationAsRead
{
    public record MarkNotificationAsReadCommand(string NotificationId) : IRequest<BaseResponse<string>>;
}
