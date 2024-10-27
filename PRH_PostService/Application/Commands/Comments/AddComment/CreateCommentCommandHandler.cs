using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Constants.AMQPMessage.Comment;
using Domain.Entities;
using MediatR;
using NUlid;
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
                Id = Ulid.NewUlid().ToString(),
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
            var comment = new Comment
            {
                CommentId = Ulid.NewUlid().ToString(),
                PostId = request.CommentDto.PostId,
                UserId = userId,
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
                    CommentRequestId = Ulid.NewUlid().ToString(),
                    PostId = comment.PostId,
                    UserId = comment.UserId,
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
