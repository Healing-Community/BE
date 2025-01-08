using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.ReportPosts.AddReport;

public record CreateReportCommand(ReportDto ReportDto) : IRequest<BaseResponse<string>>;
