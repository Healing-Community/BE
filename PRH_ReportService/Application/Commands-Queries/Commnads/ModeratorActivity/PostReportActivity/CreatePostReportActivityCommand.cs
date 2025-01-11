using System;
using Application.Commons;
using Domain.Constants.AMQPMessage.Report;
using Domain.Entities.ModeratorActivity;
using MediatR;

namespace Application.Commands_Queries.Commnads.ModeratorActivity.PostReportActivity;

public record CreatePostReportActivityCommand(BanPostMessage BanPostMessage) : IRequest<BaseResponse<string>>;
