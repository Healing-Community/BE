using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;
using Application.Commons.Tools;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.UpdateExpertProfile
{
    public class UpdateExpertProfileCommandHandler(
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateExpertProfileCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(UpdateExpertProfileCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.",
                        Field = "HttpContext"
                    });
                    response.Success = false;
                    response.StatusCode = 400;
                    return response;
                }

                // Lấy UserId và Email từ JWT token
                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                var email = Authentication.GetUserEmailFromHttpContext(httpContext);

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Không thể xác định thông tin người dùng từ token.",
                        Field = "Token"
                    });
                    response.Success = false;
                    response.StatusCode = 401;
                    return response;
                }

                // Kiểm tra nếu hồ sơ chuyên gia không tồn tại, tạo mới
                var expertProfile = await expertProfileRepository.GetByIdAsync(userId);
                if (expertProfile == null)
                {
                    expertProfile = new ExpertProfile
                    {
                        ExpertProfileId = userId,
                        UserId = userId,
                        Email = email,
                        Specialization = request.Specialization,
                        ExpertiseAreas = request.ExpertiseAreas,
                        Bio = request.Bio,
                        ProfileImageUrl = request.ProfileImageUrl,
                        Fullname = request.Fullname,
                        Status = 0, // PendingApproval
                        CreatedAt = DateTime.UtcNow.AddHours(7),
                        UpdatedAt = DateTime.UtcNow.AddHours(7)
                    };

                    await expertProfileRepository.Create(expertProfile);

                    response.Success = true;
                    response.Data = true;
                    response.StatusCode = 201; // Created
                    response.Message = "Hồ sơ chuyên gia đã được tạo mới thành công.";
                    return response;
                }

                // Cập nhật hồ sơ chuyên gia nếu đã tồn tại
                expertProfile.Specialization = request.Specialization;
                expertProfile.ExpertiseAreas = request.ExpertiseAreas;
                expertProfile.Bio = request.Bio;
                expertProfile.ProfileImageUrl = request.ProfileImageUrl;
                expertProfile.Fullname = request.Fullname;
                expertProfile.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await expertProfileRepository.Update(expertProfile.ExpertProfileId, expertProfile);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Cập nhật hồ sơ chuyên gia thành công.";
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = ex.Message,
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi xử lý hồ sơ chuyên gia.";
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
