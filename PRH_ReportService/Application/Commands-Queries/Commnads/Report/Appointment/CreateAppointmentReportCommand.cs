using System;
using Application.Commons;
using Domain.Constants.AMQPMessage.Report;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report.Appointment;

public record CreateAppointmentReportCommand(ReportAppointmentMessage ReportAppointmentMessage) : IRequest<BaseResponse<string>>;
