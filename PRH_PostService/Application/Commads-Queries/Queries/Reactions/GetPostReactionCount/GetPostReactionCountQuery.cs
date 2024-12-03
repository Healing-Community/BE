using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.Reactions.GetPostReactionCount;

public record GetPostReactionCountQuery(PostIdOnlyDto PostId) : IRequest<BaseResponse<PostReactionCountDto>>;