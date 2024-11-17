using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.Logout;

public record LogoutUserCommand(LogoutRequestDto LogoutRequestDto) : IRequest<BaseResponse<string>>;