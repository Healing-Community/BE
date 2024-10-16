using Application.Commons;
using Application.Commons.Request;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Comments.AddComment
{
    public class CreateCommentCommandHandler(ICommentRepository commentRepository)
        : IRequestHandler<CreateCommentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var commentId = NewId.NextSequentialGuid();
            var comment = new Comment
            {
                CommentId = commentId,
                PostId = request.CommentDto.PostId,
                //ParentId = request.CommentDto.ParentId,
                Content = request.CommentDto.Content,
                CreatedAt = DateTime.UtcNow                  
            };
            var response = new BaseResponse<string>
            {
                Id = commentId,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                await commentRepository.Create(comment);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Comment created successfully";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to create comment";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
