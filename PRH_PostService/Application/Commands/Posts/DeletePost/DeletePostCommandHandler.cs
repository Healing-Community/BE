using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;


namespace Application.Commands.Posts.DeletePost
{
    public class DeletePostCommandHandler(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor) 
        : IRequestHandler<DeletePostCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.Id,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };
            try
            {
                var userId = Authentication.GetUserIdFromHttpContext(httpContextAccessor.HttpContext);
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền để truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                var post = await postRepository.GetByIdAsync(request.Id);
                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Bài viết không tồn tại";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                if (post.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Bạn không có quyền xoá bài viết này";
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return response;
                }

                await postRepository.DeleteAsync(request.Id);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Xoá bài viết thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Xoá bài viết thất bại";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
