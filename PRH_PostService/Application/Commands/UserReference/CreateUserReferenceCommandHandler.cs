using System;
using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.UserReference;

public class CreateUserReferenceCommandHandler(ICategoryRepository categoryRepository,IUserReferenceRepository repository, IHttpContextAccessor accessor) : IRequestHandler<CreateUserReferenceCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateUserReferenceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userPreference = await repository.GetByPropertyAsync(x => x.UserId == userId && x.CategoryId == request.UserPreferenceDto.CategoryId);

            if (userPreference != null)
            {
                return BaseResponse<string>.SuccessReturn("User preference already exists");
            }
            // User preference does not exist, create a new one

            if (!await categoryRepository.ExistsAsync(request.UserPreferenceDto.CategoryId))
            {
                return BaseResponse<string>.NotFound("Category not found");
            }

            await repository.Create(new UserPreference
            {
                Id = Ulid.NewUlid().ToString(),
                UserId = userId,
                CategoryId = request.UserPreferenceDto.CategoryId,
            });
            return BaseResponse<string>.SuccessReturn("User preference created successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.InternalServerError(ex.Message);
        }
    }
}
