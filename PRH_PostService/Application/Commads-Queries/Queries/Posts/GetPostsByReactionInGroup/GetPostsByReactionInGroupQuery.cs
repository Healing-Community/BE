﻿using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Queries.Posts.GetPostsByReactionInGroup
{
    public record GetPostsByReactionInGroupQuery(string GroupId) : IRequest<BaseResponse<IEnumerable<PostReactionGroupDto>>>;

}
