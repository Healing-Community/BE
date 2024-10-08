using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Posts.UpdatePost
{
    public record UpdatePostCommand(Guid postId, PostDto PostDto) : IRequest<BaseResponse<string>>;
}
