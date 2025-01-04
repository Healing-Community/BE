using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Queries.Posts.GetPostsByReactionInGroup
{
    public class GetPostsByReactionInGroupQueryHandler(
    IPostRepository postRepository,
    IReactionRepository reactionRepository,
    IGroupGrpcClient groupGrpcClient,
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<GetPostsByReactionInGroupQuery, BaseResponse<IEnumerable<PostReactionGroupDto>>>
    {
        public async Task<BaseResponse<IEnumerable<PostReactionGroupDto>>> Handle(GetPostsByReactionInGroupQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy thông tin User từ HttpContext
                var userId = httpContextAccessor.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BaseResponse<IEnumerable<PostReactionGroupDto>>.Unauthorized();
                }

                // Gọi gRPC để lấy chi tiết Group
                var groupDetails = await groupGrpcClient.GetGroupDetailsAsync(request.GroupId);
                if (groupDetails == null)
                {
                    return BaseResponse<IEnumerable<PostReactionGroupDto>>.NotFound("Group không tồn tại.");
                }

                // Validate quyền truy cập vào Group
                if (groupDetails.Visibility == 1) // Private group
                {
                    var hasAccess = await groupGrpcClient.CheckUserInGroupOrPublicAsync(request.GroupId, userId);
                    if (!hasAccess)
                    {
                        return BaseResponse<IEnumerable<PostReactionGroupDto>>.Forbidden("Bạn không có quyền truy cập group này.");
                    }
                }

                // Fetch tất cả bài viết trong group
                var postsInGroup = await postRepository.GetPostsByGroupIdAsync(request.GroupId);

                if (!postsInGroup.Any())
                {
                    return BaseResponse<IEnumerable<PostReactionGroupDto>>.SuccessReturn(new List<PostReactionGroupDto>(), "Không có bài viết nào trong group.");
                }

                // Phân loại bài viết theo vai trò của người tạo bài viết
                var ownerAndModeratorPosts = new List<PostReactionGroupDto>();
                var userPosts = new List<PostReactionGroupDto>();

                foreach (var post in postsInGroup)
                {
                    // Lấy vai trò của người tạo bài viết trong group
                    var roleOfPostOwner = await groupGrpcClient.GetUserRoleInGroupAsync(request.GroupId, post.UserId ?? string.Empty);

                    // Fetch số lượng reaction
                    var reactions = await reactionRepository.GetsByPropertyAsync(r => r.PostId == post.PostId);

                    var postDto = new PostReactionGroupDto
                    {
                        PostId = post.PostId,
                        UserId = post.UserId,
                        CategoryId = post.CategoryId,
                        Title = post.Title,
                        CoverImgUrl = post.CoverImgUrl,
                        Description = post.Description,
                        Status = post.Status,
                        CreateAt = post.CreateAt,
                        UpdateAt = post.UpdateAt,
                        ReactionCount = reactions.Count(),
                        RoleInGroup = roleOfPostOwner
                    };

                    // Phân loại bài viết theo vai trò
                    if (roleOfPostOwner == "Owner" || roleOfPostOwner == "Moderator")
                    {
                        ownerAndModeratorPosts.Add(postDto);
                    }
                    else if (roleOfPostOwner == "User" && postDto.ReactionCount > 10)
                    {
                        userPosts.Add(postDto);
                    }
                }

                // Sắp xếp bài viết
                var sortedOwnerAndModeratorPosts = ownerAndModeratorPosts
                    .OrderByDescending(p => p.ReactionCount) // Sắp xếp theo reaction từ lớn đến bé
                    .ThenByDescending(p => p.CreateAt)      // Sau đó theo thời gian đăng bài mới nhất
                    .ToList();

                var sortedUserPosts = userPosts
                    .OrderByDescending(p => p.ReactionCount) // Chỉ lấy bài có reaction > 10
                    .ThenByDescending(p => p.CreateAt)
                    .ToList();

                // Gộp danh sách bài viết
                var result = sortedOwnerAndModeratorPosts.Concat(sortedUserPosts).ToList();

                return BaseResponse<IEnumerable<PostReactionGroupDto>>.SuccessReturn(result, "Lấy danh sách bài viết thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<IEnumerable<PostReactionGroupDto>>.InternalServerError($"Lỗi khi xử lý yêu cầu: {ex.Message}");
            }
        }
    }

}
