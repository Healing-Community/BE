using System;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPaymentInfo;

public record GetPaymentInfoQuery(string AppointmentId) : IRequest<BaseResponse<string>>;