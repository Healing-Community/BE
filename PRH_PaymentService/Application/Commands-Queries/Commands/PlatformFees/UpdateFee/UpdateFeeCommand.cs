using System;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commands.PlatformFees.UpdateFee;

public record UpdateFeeCommand(string PlatformFeeId,int Percent) : IRequest<BaseResponse<string>>;