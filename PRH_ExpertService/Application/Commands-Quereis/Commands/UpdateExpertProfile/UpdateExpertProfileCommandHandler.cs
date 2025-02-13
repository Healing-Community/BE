﻿using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.UpdateExpertProfile
{
    public class UpdateExpertProfileCommandHandler(
        IExpertProfileRepository expertProfileRepository,
        ICertificateRepository certificateRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateExpertProfileCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(UpdateExpertProfileCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Errors.Add("Lỗi hệ thống: không thể xác định context của yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                // Lấy UserId và Email từ JWT token
                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                var email = Authentication.GetUserEmailFromHttpContext(httpContext);

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                {
                    response.Success = false;
                    response.Errors.Add("Không thể xác định thông tin người dùng từ token.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                // Kiểm tra nếu hồ sơ chuyên gia không tồn tại
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
                    response.StatusCode = StatusCodes.Status201Created; // Created
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
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Cập nhật hồ sơ chuyên gia thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
