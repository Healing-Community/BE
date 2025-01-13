using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Domain.Enum;
using MediatR;

public class GetSideRecommendPostQueryHandler(IPostRepository repository) : IRequestHandler<GetSideRecommendPostQuery, BaseResponse<IEnumerable<PostRecommendDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostRecommendDto>>> Handle(GetSideRecommendPostQuery request, CancellationToken cancellationToken)
    {
       try
        {
            var posts = await repository.GetRandomPostsAsync(request.PageSize, request.PageNumber);
            // Map Post to PostDto in a new list
            var data = posts.Where(p=>p.Status == (int)PostStatus.Public).Select(post => new PostRecommendDto
            {
                    PostId = post.PostId,
                    UserId = post.UserId,
                    CategoryId = post.CategoryId,
                    Title = post.Title,
                    CoverImgUrl = post.CoverImgUrl,
                    Description = post.Description,
                    Status = post.Status,
                    CreateAt = post.CreateAt,
                    UpdateAt = post.UpdateAt
                });
            return BaseResponse<IEnumerable<PostRecommendDto>>.SuccessReturn(data);
        }
        catch (Exception ex) 
        {
           return BaseResponse<IEnumerable<PostRecommendDto>>.InternalServerError(ex.Message);
        }
    }
}