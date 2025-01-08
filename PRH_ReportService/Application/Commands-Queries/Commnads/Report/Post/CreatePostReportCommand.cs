using System;
using Application.Commons;
using Domain.Constants.AMQPMessage;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report;

public record CreatePostReportCommand(PostReportMessage PostReportMessage) : IRequest<BaseResponse<string>>;