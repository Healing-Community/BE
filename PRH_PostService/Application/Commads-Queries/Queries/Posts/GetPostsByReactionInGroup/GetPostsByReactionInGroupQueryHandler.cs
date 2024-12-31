using Application.Commons.DTOs;
using Application.Commons.Enum;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commads_Queries.Queries.Posts.GetPostsByReactionInGroup
{
    public class GetPostsByReactionInGroupQueryHandler(
        IReactionRepository reactionRepository,
        IPostRepository postRepository)
        : IRequestHandler<GetPostsByReactionInGroupQuery, BaseResponse<IEnumerable<PostReactionCountDto>>>
    {
        public async Task<BaseResponse<IEnumerable<PostReactionCountDto>>> Handle(GetPostsByReactionInGroupQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy các bài viết trong group
                var groupPosts = await postRepository.GetsByPropertyAsync(x => x.GroupId == request.GroupId);

                if (groupPosts == null || !groupPosts.Any())
                {
                    return BaseResponse<IEnumerable<PostReactionCountDto>>.NotFound("Không có bài viết nào trong nhóm này.");
                }

                // Lấy danh sách các bài viết ID trong group
                var postIds = groupPosts.Select(p => p.PostId).ToList();

                // Lấy các reaction của các bài viết trong group
                var reactions = await reactionRepository.GetsByPropertyAsync(x => postIds.Contains(x.PostId ?? ""), request.Top);

                if (reactions == null || !reactions.Any())
                {
                    return BaseResponse<IEnumerable<PostReactionCountDto>>.NotFound("Không có reaction nào trong bài viết của nhóm này.");
                }

                // Tạo danh sách bài viết với số lượng reaction
                var postReactionCountDtos = reactions.GroupBy(r => r.PostId).Select(group => new PostReactionCountDto
                {
                    PostId = group.Key,
                    Like = new Like
                    {
                        LikeCount = group.Count(r => r.ReactionTypeId == ((int)ReactionTypeEnum.Like).ToString()),
                        Icon = "👍"
                    },
                    Love = new Love
                    {
                        LoveCount = group.Count(r => r.ReactionTypeId == ((int)ReactionTypeEnum.Love).ToString()),
                        Icon = "❤️"
                    },
                    Haha = new Haha
                    {
                        HahaCount = group.Count(r => r.ReactionTypeId == ((int)ReactionTypeEnum.Haha).ToString()),
                        Icon = "😆"
                    },
                    Wow = new Wow
                    {
                        WowCount = group.Count(r => r.ReactionTypeId == ((int)ReactionTypeEnum.Wow).ToString()),
                        Icon = "😲"
                    },
                    Sad = new Sad
                    {
                        SadCount = group.Count(r => r.ReactionTypeId == ((int)ReactionTypeEnum.Sad).ToString()),
                        Icon = "😢"
                    },
                    Angry = new Angry
                    {
                        AngryCount = group.Count(r => r.ReactionTypeId == ((int)ReactionTypeEnum.Angry).ToString()),
                        Icon = "😡"
                    },
                    Total = group.Count()
                }).OrderByDescending(x => x.Total).Take(request.Top).ToList();

                return BaseResponse<IEnumerable<PostReactionCountDto>>.SuccessReturn(postReactionCountDtos, "Lấy danh sách bài viết theo reaction thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<IEnumerable<PostReactionCountDto>>.InternalServerError($"Lỗi xảy ra: {ex.Message}");
            }
        }
    }
}
