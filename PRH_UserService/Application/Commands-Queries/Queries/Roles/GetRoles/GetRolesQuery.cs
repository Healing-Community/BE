using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Roles.GetRoles;

public record GetRolesQuery : IRequest<BaseResponse<IEnumerable<Role>>>;