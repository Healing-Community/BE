using Application.Commons;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Users.DeleteUser
{
    public record DeleteUserCommand(Guid Id) : IRequest<BaseResponse<string>>;
}
