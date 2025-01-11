using System;
using Application.Commons;
using Domain.Constants.AMQPMessage.Report;
using MediatR;

namespace Application.Commands_Queries.Commnads.ModeratorActivity.AppointmentReportActivity;

public record CreateAppointmentReportActivityCommand(ModerateAppointmentMessage Message) : IRequest<BaseResponse<string>>;