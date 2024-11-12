using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetUsersById;

public record GetUsersByIdQuery(string id) : IRequest<BaseResponse<User>>;