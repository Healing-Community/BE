using Application.Commons;
using Domain.Entities;
using MediatR;

public record GetRoleByPropertyQuery(string Property) : IRequest<BaseResponse<Role>>;