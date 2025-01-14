using System;
using Application.Commons;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.UserReportSystem;

public record UserReportSystemCommand(string Content) : IRequest<BaseResponse<string>>;