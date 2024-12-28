using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System.Net;
using Application.Commons.Tools;
using Microsoft.AspNetCore.Http;
using NUlid;


namespace Application.Commads_Queries.Queries.Posts.GetPostsInGroups
{
    public class GetPostsInGroupsQueryHandler(
    IPostRepository postRepository,
    IGroupGrpcClient groupGrpcClient)
    : IRequestHandler<GetPostsInGroupsQuery, BaseResponse<List<PostGroupDetailDto>>>
    {
        public async Task<BaseResponse<List<PostGroupDetailDto>>> Handle(GetPostsInGroupsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<PostGroupDetailDto>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);

                // Fetch posts in groups with validation
                var posts = await postRepository.GetAllPostsInGroupsWithValidationAsync(
                    userId,
                    groupGrpcClient.GetGroupDetailsAsync,
                    groupGrpcClient.CheckUserInGroupAsync
                );

                if (!posts.Any())
                {
                    response.Data = new List<PostGroupDetailDto>();
                    response.Success = true;
                    response.Message = "Không có bài viết nào trong nhóm.";
                    response.StatusCode = (int)HttpStatusCode.OK;
                    return response;
                }

                response.Data = posts.Select(p => new PostGroupDetailDto
                {
                    PostId = p.PostId,
                    GroupId = p.GroupId,
                    CategoryId = p.CategoryId,
                    Title = p.Title,
                    CoverImgUrl = p.CoverImgUrl,
                    Description = p.Description,
                    Status = p.Status,
                    CreateAt = p.CreateAt,
                    UpdateAt = p.UpdateAt,
                    UserId = p.UserId,                   
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
