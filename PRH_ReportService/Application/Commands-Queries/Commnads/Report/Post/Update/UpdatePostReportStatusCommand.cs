using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report.Post.Update;

public record UpdatePostReportStatusCommand(string PostId, bool IsApprove) : IRequest<BaseResponse<string>>;