using Application.Commons;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Comments.DeleteComment
{
    public record DeleteCommentCommand(Guid Id) : IRequest<BaseResponse<string>>;
}
