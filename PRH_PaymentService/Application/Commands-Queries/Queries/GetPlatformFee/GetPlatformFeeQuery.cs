using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPlatformFee;

public record GetPlatformFeeQuery : IRequest<BaseResponse<PlatformFee>>;
