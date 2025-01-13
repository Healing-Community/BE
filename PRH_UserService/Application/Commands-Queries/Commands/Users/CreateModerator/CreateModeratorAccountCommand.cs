using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.CreateModerator;

public record CreateModeratorAccountCommand(RegisterModeratorAccountDto registerModeratorAccountDto) : IRequest<DetailBaseResponse<string>>;