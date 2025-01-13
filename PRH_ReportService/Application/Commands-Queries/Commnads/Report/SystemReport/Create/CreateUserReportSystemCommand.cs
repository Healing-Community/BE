using System;
using Application.Commons;
using Domain.Constants.AMQPMessage;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report.SystemReport;

public record CreateUserReportSystemCommand(UserReportSystemMessage UserReportSystemMessage) : IRequest<BaseResponse<string>>;
