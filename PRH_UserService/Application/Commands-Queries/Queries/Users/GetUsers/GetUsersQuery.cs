using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetUsers;

public record GetUsersQuery : IRequest<BaseResponse<IEnumerable<User>>>;