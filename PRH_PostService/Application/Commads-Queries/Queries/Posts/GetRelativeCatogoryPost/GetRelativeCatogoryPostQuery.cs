using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commads_Queries.Queries.Posts.GetRelativeCatogoryPost;

public record GetRelativeCatogoryPostQuery(string PostId, int Top) : IRequest<BaseResponse<IEnumerable<PostRecommendDto>>>;
