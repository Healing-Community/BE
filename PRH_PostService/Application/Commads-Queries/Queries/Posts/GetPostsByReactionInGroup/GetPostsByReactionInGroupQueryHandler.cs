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

                // Validate quyền truy cập
                if (groupDetails.Visibility == 1) // Private group
                {
                    var hasAccess = await groupGrpcClient.CheckUserInGroupOrPublicAsync(request.GroupId, userId);
                    if (!hasAccess)
                    {
                        return BaseResponse<IEnumerable<PostReactionGroupDto>>.Forbidden("Bạn không có quyền truy cập group này.");
                    }
                }

                // Fetch all posts in the group
                var postsInGroup = await postRepository.GetPostsByGroupIdAsync(request.GroupId);

                // Fetch all reactions for posts in the group
                var reactions = await reactionRepository.GetsByPropertyAsync(r => postsInGroup.Select(p => p.PostId).Contains(r.PostId));

                // Group reactions by PostId
                var reactionsGrouped = reactions.GroupBy(r => r.PostId)
                    .Select(g => new
                    {
                        PostId = g.Key,
                        ReactionCount = g.Count()
                    }).ToList();

                // Combine posts and reaction counts
                var postDtos = postsInGroup
                    .Select(post =>
                    {
                        var reactionData = reactionsGrouped.FirstOrDefault(r => r.PostId == post.PostId);
                        return new PostReactionGroupDto
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
                            ReactionCount = reactionData?.ReactionCount ?? 0
                        };
                    })
                    .OrderByDescending(p => p.ReactionCount >= 10) 
                    .ThenByDescending(p => p.ReactionCount) 
                    .ThenBy(p => p.CreateAt) 
                    .ToList();
                return BaseResponse<IEnumerable<PostReactionGroupDto>>.SuccessReturn(postDtos, "Lấy danh sách bài viết thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<IEnumerable<PostReactionGroupDto>>.InternalServerError($"Lỗi khi xử lý yêu cầu: {ex.Message}");
            }
        }
    }
}
