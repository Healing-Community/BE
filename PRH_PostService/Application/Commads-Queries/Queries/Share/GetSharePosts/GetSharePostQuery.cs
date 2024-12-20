using Application.Commons;
using Application.Commons.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Commads_Queries.Queries.Share;

public record GetSharePostQuery : IRequest<BaseResponse<IEnumerable<PostDetailDto>>>;