using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commads_Queries.Queries.Posts.GetOtherPostByAutour;

public record GetOtherPostByAutourQuery(string AuthourId, int Top) : IRequest<BaseResponse<IEnumerable<PostRecommendDto>>>;