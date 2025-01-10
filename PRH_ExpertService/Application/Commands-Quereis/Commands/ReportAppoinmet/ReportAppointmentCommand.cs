using Application.Commons;
using MediatR;

namespace Application.Commands_Quereis.Commands.ReportAppoinmet;

public record ReportAppointmentCommand (string AppoinmtentId, string ReportDescription) :IRequest<BaseResponse<string>>;