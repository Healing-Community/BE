using Application.Commons;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.NotifyFollowers
{
    public record NotifyFollowersCommand(Guid UserId, Guid PostId, string PostTitle) : IRequest<BaseResponse<string>>;
}
