using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.GetExpertAvailbilityByAppointmentId;

public record GetExpertAvailbilityByAppointmentIdQuery(string AppointmentId) : IRequest<BaseResponse<ExpertAvailabilityResponseDto>>;