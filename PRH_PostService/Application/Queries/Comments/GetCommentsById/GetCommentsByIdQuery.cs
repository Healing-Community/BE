using Application.Commons;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Comments.GetCommentsById
{
    public record GetCommentsByIdQuery(Guid Id) : IRequest<BaseResponse<Comment>>;
}
