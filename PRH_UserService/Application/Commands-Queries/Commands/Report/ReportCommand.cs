using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Report;

public record ReportCommand(ReportMessageDto ReportMessageDto) : IRequest<BaseResponse<string>>;