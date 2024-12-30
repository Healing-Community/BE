using Application.Commons.Tools;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;


namespace Application.Commads_Queries.Commands.Posts.UpdatePostInGroup
{
    public class UpdatePostInGroupCommandHandler(
            IPostRepository postRepository,
            ICategoryRepository categoryRepository,
            IHttpContextAccessor httpContextAccessor,
            IGroupGrpcClient groupGrpcClient)
            : IRequestHandler<UpdatePostInGroupCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdatePostInGroupCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.PostId,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy UserId từ HttpContext
                var userId = Authentication.GetUserIdFromHttpContext(httpContextAccessor.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Kiểm tra bài viết có tồn tại
                var existingPost = await postRepository.GetByIdAsync(request.PostId);
                if (existingPost == null)
                {
                    response.Success = false;
                    response.Message = "Bài viết không tồn tại";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Kiểm tra quyền sở hữu bài viết
                if (existingPost.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Bạn không có quyền cập nhật bài viết này";
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return response;
                }

                // Kiểm tra bài viết có thuộc nhóm hay không
                if (string.IsNullOrEmpty(existingPost.GroupId))
                {
                    response.Success = false;
                    response.Message = "Bài viết này không thuộc nhóm nào";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                // Kiểm tra nhóm có tồn tại hay không
                var groupExists = await groupGrpcClient.CheckGroupExistsAsync(existingPost.GroupId);
                if (!groupExists)
                {
                    response.Success = false;
                    response.Message = "Nhóm không tồn tại";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                // Kiểm tra danh mục có tồn tại không
                if (!await categoryRepository.ExistsAsync(request.PostDto.CategoryId))
                {
                    response.Success = false;
                    response.Message = "Danh mục không tồn tại";
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                // Cập nhật bài viết nhưng giữ nguyên GroupId
                existingPost.Title = request.PostDto.Title;
                existingPost.CategoryId = request.PostDto.CategoryId;
                existingPost.CoverImgUrl = request.PostDto.CoverImgUrl;
                existingPost.Description = request.PostDto.Description;
                existingPost.UpdateAt = DateTime.UtcNow.AddHours(7);

                await postRepository.Update(existingPost.PostId, existingPost);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Cập nhật bài viết thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Cập nhật bài viết thất bại";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
