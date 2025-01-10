using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Report.Appointment;

public record GetAppointmentReportQuery : IRequest<BaseResponse<IEnumerable<AppointmentReport>>>;
