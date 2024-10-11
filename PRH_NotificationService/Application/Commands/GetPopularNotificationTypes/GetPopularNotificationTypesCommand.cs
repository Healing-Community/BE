using Application.Commons;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.GetPopularNotificationTypes
{
    public record GetPopularNotificationTypesCommand() : IRequest<BaseResponse<Dictionary<string, int>>>;
}
