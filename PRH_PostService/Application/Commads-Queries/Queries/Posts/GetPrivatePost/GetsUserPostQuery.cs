using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.Posts.GetPrivatePost;

public record GetsUserPostQuery(string UserId, int PageNumber, int PageSize) : IRequest<BaseResponse<IEnumerable<PostRecommendDto>>>;