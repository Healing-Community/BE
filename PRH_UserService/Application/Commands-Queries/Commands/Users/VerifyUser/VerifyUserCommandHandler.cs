using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.VerifyUser;

public class VerifyUserCommandHandler(ITokenService tokenService, IUserRepository userRepository)
    : IRequestHandler<VerifyUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow.AddHours(7)
        };

        try
        {
            var (userId, isValidated) = tokenService.ValidateToken(request.Token);

            if ((userId, isValidated) != default)
            {
                if (isValidated && userId != null)
                {
                    // Tìm người dùng theo ID
                    var user = await userRepository.GetByIdAsync(userId);
                    if (user == null)
                    {
                        response.Success = false;
                        response.Message = "Không tìm thấy người dùng.";
                        response.StatusCode = 404;
                        response.Errors = ["Không tìm thấy người dùng."];
                        return response;
                    }

                    // Cập nhật trạng thái người dùng thành đã xác minh
                    user.Status = 1;
                    await userRepository.UpdateAsync(user.UserId, user);

                    // Thiết lập chi tiết phản hồi thành công
                    response.Success = true;
                    response.Message = "Xác minh email thành công.";
                    response.StatusCode = 200;
                }
                else if (!isValidated && userId != null)
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
            response.Message = "Lỗi không xác định.";
            response.StatusCode = 500;
            response.Errors = [ex.Message];
        }

        return response;
    }
}