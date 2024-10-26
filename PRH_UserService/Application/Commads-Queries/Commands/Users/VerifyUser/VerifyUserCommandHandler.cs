using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands.Users.VerifyUser;

public class VerifyUserCommandHandler(ITokenService tokenService, IUserRepository userRepository)
    : IRequestHandler<VerifyUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow
        };

        try
        {
            
            var (userId,isValidated) = tokenService.ValidateToken(request.Token);

            if ((userId, isValidated) != default)
            {
                if (isValidated)
                {
                    // Tìm người dùng theo ID
                    var user = await userRepository.GetByIdAsync(userId);
                    if (user == null)
                    {
                        response.Success = false;
                        response.Message = "Không tìm thấy người dùng.";
                        response.StatusCode = 404;
                        response.Errors = new List<string> { "Không tìm thấy người dùng." };
                        return response;
                    }

                    // Cập nhật trạng thái người dùng thành đã xác minh
                    user.Status = 1;
                    await userRepository.Update(user.UserId, user);

                    // Thiết lập chi tiết phản hồi thành công
                    response.Success = true;
                    response.Message = $"Xác minh email thành công.";
                    response.StatusCode = 200;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Xác minh email thất bại.";
                    response.StatusCode = 400;
                    response.Errors = ["Token không hợp lệ."];
                    await userRepository.DeleteAsync(userId);
                }
            }
           
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Lỗi xác định.";
            response.StatusCode = 500;
            response.Errors = new List<string> { ex.Message };
        }

        return response;
    }
}
