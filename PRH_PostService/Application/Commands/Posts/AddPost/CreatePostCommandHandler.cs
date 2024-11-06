using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Constants.AMQPMessage.Post;
using Domain.Entities;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.Posts.AddPost
{
    public class CreatePostCommandHandler(IMessagePublisher messagePublisher, IPostRepository postRepository, IGroupGrpcClient groupGrpcClient)
        : IRequestHandler<CreatePostCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy UserId từ HttpContext
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Kiểm tra sự tồn tại của Group nếu GroupId được cung cấp
                if (!string.IsNullOrEmpty(request.PostDto.GroupId))
                {
                    var groupExists = await groupGrpcClient.CheckGroupExistsAsync(request.PostDto.GroupId);
                    if (!groupExists)
                    {
                        response.Success = false;
                        response.Message = "Nhóm không tồn tại.";
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return response;
                    }
                }

                // Tạo bài viết
                var post = new Post
                {
                    PostId = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    GroupId = request.PostDto.GroupId, // Nếu là bài đăng trong nhóm thì GroupId sẽ có giá trị
                    CategoryId = request.PostDto.CategoryId,
                    Title = request.PostDto.Title,
                    CoverImgUrl = request.PostDto.CoverImgUrl,
                    VideoUrl = request.PostDto.VideoUrl,
                    Description = request.PostDto.Description,
                    Status = request.PostDto.Status,
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    UpdateAt = DateTime.UtcNow.AddHours(7)
                };

                await postRepository.Create(post);
                response.StatusCode = (int)HttpStatusCode.Created;
                response.Success = true;
                response.Message = "Tạo bài viết thành công";
                response.Data = post.PostId;

                // Gửi thông báo đến Queue để xử lý tiếp (nếu cần)
                var postingRequestCreatedMessage = new PostingRequestCreatedMessage
                {
                    PostingRequestId = post.PostId,
                    UserId = post.UserId,
                    Tittle = post.Title,
                    PostedDate = post.CreateAt
                };
                await messagePublisher.PublishAsync(postingRequestCreatedMessage, QueueName.PostQueue, cancellationToken);
            }
            catch (Grpc.Core.RpcException grpcEx)
            {
                response.StatusCode = (int)HttpStatusCode.BadGateway;
                response.Success = false;
                response.Message = "Lỗi kết nối với Group Service";
                response.Errors.Add(grpcEx.Message);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình tạo bài viết";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
