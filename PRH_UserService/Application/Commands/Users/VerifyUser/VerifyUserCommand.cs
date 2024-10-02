using Application.Commons;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Users.VerifyUser
{
    public record VerifyUserCommand(string Token) : IRequest<BaseResponse<string>>;
}
