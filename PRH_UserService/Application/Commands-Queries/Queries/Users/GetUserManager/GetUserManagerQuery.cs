using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetUserManager;

public record GetUserManagerQuery : IRequest<BaseResponse<IEnumerable<UserDto>>>;