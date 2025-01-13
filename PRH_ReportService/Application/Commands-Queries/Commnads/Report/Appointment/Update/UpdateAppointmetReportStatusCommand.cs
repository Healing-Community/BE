using System;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report.Appointment.Update;

public record UpdateAppointmetReportStatusCommand (string AppointmentId, bool IsApprove) : IRequest<BaseResponse<string>>;