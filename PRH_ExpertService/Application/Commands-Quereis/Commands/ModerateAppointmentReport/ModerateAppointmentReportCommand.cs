using Application.Commons;
using MediatR;

namespace Application.Commands_Quereis.Commands.ModerateAppointmentReport;

public record ModerateAppointmentReportCommand(string AppointmentId, bool IsApprove) : IRequest<BaseResponse<string>>;
