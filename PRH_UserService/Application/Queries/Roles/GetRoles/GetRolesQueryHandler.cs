﻿using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;

namespace Application.Queries.Roles.GetRoles;

public class GetRolesQueryHandler(IRoleRepository roleRepository)
    : IRequestHandler<GetRolesQuery, BaseResponse<IEnumerable<Role>>>
{
    public async Task<BaseResponse<IEnumerable<Role>>> Handle(GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var response = new BaseResponse<IEnumerable<Role>>
        {
            Id = NewId.NextSequentialGuid(),
            Timestamp = DateTime.UtcNow
        };
        try
        {
            var roles = await roleRepository.GetsAsync();
            response.Message = "Roles retrieved successfully";
            response.Success = true;
            response.Data = roles;
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.Success = false;
        }

        return response;
    }
}