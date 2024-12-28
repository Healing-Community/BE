

using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System.Net;
using Domain.Enum;
using NUlid;

namespace Application.Commads_Queries.Queries.Posts.GetPostsInGroupByUserAndGroup
{
    public class GetPostsInGroupByUserAndGroupQueryHandler(
            IPostRepository postRepository,
            IGroupGrpcClient groupGrpcClient
        ) : IRequestHandler<GetPostsInGroupByUserAndGroupQuery, BaseResponse<List<PostGroupWithoutGroupIDAndUserID>>>
    {
        public async Task<BaseResponse<List<PostGroupWithoutGroupIDAndUserID>>> Handle(GetPostsInGroupByUserAndGroupQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<PostGroupWithoutGroupIDAndUserID>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var authenticatedUserId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (authenticatedUserId == null)
                {
                    response.Success = false;
                    response.Message = "Người dùng không có quyền để truy cập.";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Validate the existence of the group
                var groupExists = await groupGrpcClient.CheckGroupExistsAsync(request.GroupId);
                if (!groupExists)
                {
                    response.Success = false;
                    response.Message = "Nhóm không tồn tại.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Check if the user has access to the group
                var hasAccess = await groupGrpcClient.CheckUserInGroupOrPublicAsync(request.GroupId, request.UserId);
                if (!hasAccess)
                {
                    response.Success = false;
                    response.Message = "Người dùng không có quyền truy cập vào nhóm này.";
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return response;
                }

                // Fetch posts by group ID
                var posts = await postRepository.GetsByPropertyAsync(
                    p => p.GroupId == request.GroupId && p.Status == (int)PostStatus.Group
                );

                if (!posts.Any())
                {
                    response.Success = true;
                    response.Message = "Không có bài viết nào trong nhóm.";
                    response.Data = new List<PostGroupWithoutGroupIDAndUserID>();
                    response.StatusCode = (int)HttpStatusCode.OK;
                    return response;
                }

                response.Data = posts.Select(p => new PostGroupWithoutGroupIDAndUserID
                {
                    PostId = p.PostId,
                    CategoryId = p.CategoryId,
                    Title = p.Title,
                    Description = p.Description,
                    CoverImgUrl = p.CoverImgUrl,
                    CreateAt = p.CreateAt,
                    UpdateAt = p.UpdateAt,
                    Status = p.Status,
                }).ToList();

                response.Success = true;
                response.Message = "Lấy danh sách bài viết trong nhóm thành công.";
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách bài viết.";
                response.Errors.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}
