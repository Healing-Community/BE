using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetCountRegistrationDays
{
    public class CountRegistrationDaysQueryHandler(IUserRepository userRepository)
        : IRequestHandler<CountRegistrationDaysQuery, BaseResponse<UserRegistrationDaysDto>>
    {
        public async Task<BaseResponse<UserRegistrationDaysDto>> Handle(CountRegistrationDaysQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = request.UserId;

                // Tìm user từ repository
                var user = await userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return BaseResponse<UserRegistrationDaysDto>.NotFound("Người dùng không tồn tại.");
                }

                // Tính tổng số ngày đã đăng ký
                var totalDays = (DateTime.UtcNow.AddHours(7) - user.CreatedAt).Days;

                var result = new UserRegistrationDaysDto
                {
                    UserId = userId,
                    TotalDays = totalDays
                };

                return BaseResponse<UserRegistrationDaysDto>.SuccessReturn(result, "Tổng số ngày đăng ký tài khoản được tính thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<UserRegistrationDaysDto>.InternalServerError($"Lỗi khi tính số ngày đăng ký tài khoản: {ex.Message}");
            }
        }
    }

}
