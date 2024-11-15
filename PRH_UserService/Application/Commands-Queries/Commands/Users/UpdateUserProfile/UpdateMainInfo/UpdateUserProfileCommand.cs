using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.UpdateUserProfile;

public record UpdateUserProfileCommand(UpdateUserDto UserDto) : IRequest<DetailBaseResponse<string>>;