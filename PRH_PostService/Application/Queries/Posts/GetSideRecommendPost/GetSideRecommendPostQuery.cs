using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

public record GetSideRecommendPostQuery(int PageSize, int PageNumber): IRequest<BaseResponse<IEnumerable<PostRecommendDto>>>;