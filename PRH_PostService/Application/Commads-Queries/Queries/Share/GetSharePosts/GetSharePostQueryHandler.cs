using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Domain.Enum;
using MediatR;

namespace Application.Commads_Queries.Queries.Share;

public class GetSharePostQueryHandler(IShareRepository shareRepository,IPostRepository postRepository) : IRequestHandler<GetSharePostQuery, BaseResponse<IEnumerable<PostDetailShareDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostDetailShareDto>>> Handle(GetSharePostQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = request.UserId;
            var posts = await shareRepository.GetsByPropertyAsync(s=> s.UserId == userId && s.Platform == "Internal");
            var postList = posts?.ToList();
            foreach (var item in postList ?? [])
            {
                item.Post = await postRepository.GetByIdAsync(item.PostId);
            }
            var postDetailDtos = postList?.Select(p => new PostDetailShareDto
            {
                ShareAt = p.CreatedAt,
                ShareId = p.ShareId,
                ShareDescription = p.Description,
                PostId = p.PostId,
                Title = p.Post.Title,
                CategoryId = p.Post.CategoryId,
                UserId = p.Post.UserId,
                CoverImgUrl = p.Post.CoverImgUrl,
                CreateAt = p.Post.CreateAt,
                Description = p.Post.Description,
                GroupId = p.Post.GroupId,
                Status = p.Post.Status,
                UpdateAt = p.Post.UpdateAt
            });
            if (postDetailDtos == null) return BaseResponse<IEnumerable<PostDetailShareDto>>.SuccessReturn([]);
            return BaseResponse<IEnumerable<PostDetailShareDto>>.SuccessReturn(postDetailDtos.Where(p => p.Status == (int)PostStatus.Public));
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PostDetailShareDto>>.InternalServerError(e.Message);
            throw;
        }
    }
}
