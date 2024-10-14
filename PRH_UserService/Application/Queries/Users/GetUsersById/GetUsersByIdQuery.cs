using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Users.GetUsersById;

public record GetUsersByIdQuery(Guid id) : IRequest<BaseResponse<User>>;