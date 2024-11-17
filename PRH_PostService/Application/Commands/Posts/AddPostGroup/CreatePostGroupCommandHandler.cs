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

namespace Application.Commands.Posts.AddPostGroup
{
    public class CreatePostGroupCommandHandler(
        IMessagePublisher messagePublisher, 
        IPostRepository postRepository, 
        IGroupGrpcClient groupGrpcClient, 
        ICategoryRepository categoryRepository)
        : IRequestHandler<CreatePostGroupCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreatePostGroupCommand request, CancellationToken cancellationToken)
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

                // Validate CategoryId
                if (string.IsNullOrEmpty(request.PostGroupDto.CategoryId))
                {
                    response.Success = false;
                    response.Message = "CategoryId không được bỏ trống.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                // Kiểm tra sự tồn tại của Category
                var categoryExists = await categoryRepository.ExistsAsync(request.PostGroupDto.CategoryId);
                if (!categoryExists)
                {
                    response.Success = false;
                    response.Message = "CategoryId không tồn tại.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }
                // Kiểm tra sự tồn tại của Group nếu GroupId được cung cấp
                if (!string.IsNullOrEmpty(request.PostGroupDto.GroupId))
                {
                    var groupExists = await groupGrpcClient.CheckGroupExistsAsync(request.PostGroupDto.GroupId);
                    if (!groupExists)
                    {
                        response.Success = false;
                        response.Message = "Nhóm không tồn tại.";
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return response;
                    }
                }

                // Validate Title
                if (string.IsNullOrEmpty(request.PostGroupDto.Title))
                {
                    response.Success = false;
                    response.Message = "Tiêu đề không được bỏ trống.";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                // Tạo bài viết
                var post = new Post
                {
                    PostId = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    GroupId = request.PostGroupDto.GroupId, 
                    CategoryId = request.PostGroupDto.CategoryId,
                    Title = request.PostGroupDto.Title,
                    CoverImgUrl = request.PostGroupDto.CoverImgUrl,
                    VideoUrl = request.PostGroupDto.VideoUrl,
                    Description = request.PostGroupDto.Description,
                    Status = request.PostGroupDto.Status,
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
