using Application.Commons.DTOs;
using Application.Commons;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Users.ResetPassword
{
    public record ResetPasswordCommand(ResetPasswordDto ResetPasswordDto) : IRequest<BaseResponse<string>>;
}
