using Application.Commons.DTOs;
using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Queries.Posts.GetPostsInGroupByUserAndGroup
{
    public record GetPostsInGroupByUserAndGroupQuery(string GroupId, string UserId, HttpContext HttpContext)
        : IRequest<BaseResponse<List<PostGroupWithoutGroupIDAndUserID>>>;
}
