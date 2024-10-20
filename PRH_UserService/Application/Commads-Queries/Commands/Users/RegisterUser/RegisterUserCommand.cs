using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Users.RegisterUser;

public record RegisterUserCommand(RegisterUserDto RegisterUserDto, string BaseUrl) : IRequest<BaseResponse<string>>;