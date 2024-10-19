using Application.Commons;
using Application.Commons.Request.Comment;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Entities;
using MassTransit;
using MediatR;
using System.Net;

namespace Application.Commands.Comments.AddComment
{
    public class CreateCommentCommandHandler(ICommentRepository commentRepository, IMessagePublisher messagePublisher)
        : IRequestHandler<CreateCommentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
            if (userId == null)
            {
                response.Success = false;
                response.Message = "Không có quyền để truy cập";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return response;
            }
            var userGuid = Guid.Parse(userId);
            var comment = new Comment
            {
                CommentId = NewId.NextSequentialGuid(),
                PostId = request.CommentDto.PostId,
                UserId = userGuid,
                //ParentId = request.CommentDto.ParentId,
                Content = request.CommentDto.Content,
                CreatedAt = DateTime.UtcNow                  
            };
            try
            {
                await commentRepository.Create(comment);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Tạo bình luận thành công";
                // Send the Request to the Queue for processing
                var commentRequestCreatedMessage = new CommentRequestCreatedMessage
                {
                    CommentRequestId = NewId.NextSequentialGuid(),
                    PostId = comment.PostId,
                    UserId = userGuid,
                    Content = comment.Content,
                    CommentedDate = comment.CreatedAt
                };
                await messagePublisher.PublishAsync(commentRequestCreatedMessage, QueueName.CommentQueue, cancellationToken);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Không tạo được bình luận";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
