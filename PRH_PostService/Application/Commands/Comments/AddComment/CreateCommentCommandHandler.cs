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
    public class CreateCommentCommandHandler(
        ICommentRepository commentRepository, 
        IMessagePublisher messagePublisher, 
        IPostRepository postRepository)
        : IRequestHandler<CreateCommentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Kiểm tra postId hợp lệ
                var post = await postRepository.GetByIdAsync(request.CommentDto.PostId);
                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Post không tồn tại.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                // Xử lý parentId
                if (!string.IsNullOrEmpty(request.CommentDto.ParentId))
                {
                    // Kiểm tra parentId là một comment hợp lệ
                    var parentComment = await commentRepository.GetByIdAsync(request.CommentDto.ParentId);
                    if (parentComment == null)
                    {
                        response.Success = false;
                        response.Message = "Parent comment không tồn tại.";
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return response;
                    }
                }

                var comment = new Comment
                {
                    CommentId = Ulid.NewUlid().ToString(),
                    PostId = request.CommentDto.PostId,
                    ParentId = request.CommentDto.ParentId, 
                    UserId = userId,
                    Content = request.CommentDto.Content,
                    CoverImgUrl = request.CommentDto.CoverImgUrl,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await commentRepository.Create(comment);
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Success = true;
                response.Message = "Tạo bình luận thành công";
                response.Data = comment.CommentId;

                // Send the Request to the Queue for processing
                var commentRequestCreatedMessage = new CommentRequestCreatedMessage
                {
                    CommentRequestId = comment.CommentId,
                    PostId = comment.PostId,
                    ParentId = comment.ParentId,
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
