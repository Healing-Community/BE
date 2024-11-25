using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

public record GetRecommendedPostsQuery(int PageNumber, int PageSize): IRequest<BaseResponse<IEnumerable<PostDto>>>;