using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Users.UpdateUser
{
    public class UpdateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.Id,
                Timestamp = DateTime.UtcNow
            };
            try
            {
                var existingUser = await userRepository.GetByIdAsync(request.Id);
                var updatedUser = new User
                {
                    Id = request.Id,
                    Email = request.UserDto.Email,
                    FullName = request.UserDto.FullName,
                    Status = request.UserDto.Status,
                    UserName = request.UserDto.UserName,
                    PasswordHash = request.UserDto.PasswordHash,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    RoleId = request.UserDto.RoleId,
                };
                await userRepository.Update(request.Id, updatedUser);
                response.Success = true;
                response.Message = "User updated successfully";
                response.Errors = Enumerable.Empty<string>();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to update user";
                response.Errors = new[] { ex.Message };
            }

            return response;
        }
    }
}
