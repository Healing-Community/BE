using Application.CommandsQueries.Queries.Posts.GetsTopPost;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commads_Queries.Queries.Posts.GetsTopPost;

public class GetsTopPostQueryHandler(IPostRepository postRepository, IReactionRepository reactionRepository) : IRequestHandler<GetsTopPostQuery, BaseResponse<IEnumerable<PostRecommendDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostRecommendDto>>> Handle(GetsTopPostQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var topReactionPosts = await reactionRepository.GetsMostReactedPost(request.Top);
            var topPosts = new List<PostRecommendDto>();
            foreach (var post in topReactionPosts)
            {
                var postEntity = await postRepository.GetByIdAsync(post.PostId);
                var postDto = new PostRecommendDto
                {
                    PostId = postEntity.PostId,
                    Title = postEntity.Title,
                    CategoryId = postEntity.CategoryId,
                    CoverImgUrl = postEntity.CoverImgUrl,
                    CreateAt = postEntity.CreateAt,
                    Description = postEntity.Description,
                    UserId = postEntity.UserId,
                    Status = postEntity.Status,
                    UpdateAt = postEntity.UpdateAt
                };
                topPosts.Add(postDto);
            }
            return BaseResponse<IEnumerable<PostRecommendDto>>.SuccessReturn(topPosts);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PostRecommendDto>>.InternalServerError(e.Message);
        }
    }
}