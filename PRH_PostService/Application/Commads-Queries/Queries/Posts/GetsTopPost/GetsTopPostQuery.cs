using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.CommandsQueries.Queries.Posts.GetsTopPost;
public record GetsTopPostQuery(int Top) : IRequest<BaseResponse<IEnumerable<PostRecommendDto>>>;