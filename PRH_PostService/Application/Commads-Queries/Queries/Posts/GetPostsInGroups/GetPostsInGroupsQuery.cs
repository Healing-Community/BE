﻿using Application.Commons.DTOs;
using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Queries.Posts.GetPostsInGroups
{
    public record GetPostsInGroupsQuery(HttpContext HttpContext) : IRequest<BaseResponse<List<PostGroupDetailDto>>>;
}
