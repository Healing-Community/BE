﻿using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Queries.GetExpertRevenueDetails;

public record GetExpertRevenueDetailsQuery : IRequest<BaseResponse<IEnumerable<ExpertRevenueDetailsDto>>>
{
    public string FilterType { get; set; } // "day", "week", "month", "year"
}
