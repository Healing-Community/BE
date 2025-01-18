using System;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Domain.Enum;
using MediatR;

namespace Application.Commads_Queries.Queries.Posts.GetOtherPostByAutour;

public class GetOtherPostByAutourQueryHandler(IPostRepository postRepository) : IRequestHandler<GetOtherPostByAutourQuery, BaseResponse<IEnumerable<PostRecommendDto>>>
{
    public async Task<BaseResponse<IEnumerable<PostRecommendDto>>> Handle(GetOtherPostByAutourQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var otherPostByAuthour = await postRepository.GetsByPropertyAsync(x => x.UserId == request.AuthourId && x.Status != (int)PostStatus.Baned && x.Status != (int)PostStatus.Group, request.Top);
            var otherPosts = new List<PostRecommendDto>();
            foreach (var post in otherPostByAuthour)
            {
                var postDto = new PostRecommendDto
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    CategoryId = post.CategoryId,
                    CoverImgUrl = post.CoverImgUrl,
                    CreateAt = post.CreateAt,
                    Description = post.Description,
                    UserId = post.UserId,
                    Status = post.Status,
                    UpdateAt = post.UpdateAt
                };
                otherPosts.Add(postDto);
            }
            return BaseResponse<IEnumerable<PostRecommendDto>>.SuccessReturn(otherPosts);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<PostRecommendDto>>.InternalServerError(e.Message);
        }
    }
}