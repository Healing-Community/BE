using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enum;
using MediatR;

namespace Application.Commads_Queries.Queries.Posts.GetRelativeCatogoryPost;

public class GetRelativeCatogoryPostQueryHandler(IPostRepository postRepository) : IRequestHandler<GetRelativeCatogoryPostQuery, BaseResponse<IEnumerable<PostRecommendDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostRecommendDto>>> Handle(GetRelativeCatogoryPostQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var post = await postRepository.GetByIdAsync(request.PostId);
            // Get all post in the same category except the current post
            var relativeCategoryPost = await postRepository.GetsByPropertyAsync(
                x => x.CategoryId == post.CategoryId && x.Status == (int)PostStatus.Public && x.PostId != post.PostId,
                request.Top);
            // Remove the current post from the list
            relativeCategoryPost = relativeCategoryPost?.Where(x => x.PostId != post.PostId);
            // Mapping Post to PostRecommendDto
            var relativeCategoryPostDto = relativeCategoryPost?.Select(x => new PostRecommendDto
            {
                PostId = x.PostId,
                Title = x.Title,
                CategoryId = x.CategoryId,
                CoverImgUrl = x.CoverImgUrl,
                CreateAt = x.CreateAt,
                Description = x.Description,
                UserId = x.UserId,
                Status = x.Status,
                UpdateAt = x.UpdateAt
            });
            return BaseResponse<IEnumerable<PostRecommendDto>>.SuccessReturn(relativeCategoryPostDto);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PostRecommendDto>>.InternalServerError(e.Message);
            throw;
        }
    }
}
