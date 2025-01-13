using System;
using Application.Commons;
using Domain.Constants.AMQPMessage.Report;
using MediatR;

namespace Application.Commands_Queries.Commnads.ModeratorActivity.CommentReportActivity;

public record CreateCommentReportActivityCommand(BanCommentMessage BanCommentMessage) : IRequest<BaseResponse<string>>;