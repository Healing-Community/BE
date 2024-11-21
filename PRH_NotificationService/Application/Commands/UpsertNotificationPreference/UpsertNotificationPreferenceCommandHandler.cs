using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.UpdateNotificationPreference
{
    public class UpsertNotificationPreferenceCommandHandler(IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
        INotificationTypeRepository notificationTypeRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpsertNotificationPreferenceCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpsertNotificationPreferenceCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
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
                    response.Success = false;
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy ID người dùng.";
                    response.StatusCode = 400;
                    return response;
                }

                var notificationType = await notificationTypeRepository.GetByIdAsync(request.NotificationTypeId);
                if (notificationType == null)
                {
                    response.Success = false;
                    response.Message = $"Không tìm thấy loại thông báo với ID '{request.NotificationTypeId}'. Vui lòng kiểm tra lại.";
                    response.StatusCode = 404;
                    return response;
                }

                var preference = await userNotificationPreferenceRepository.GetByUserIdAndNotificationTypeIdAsync(userId, notificationType.NotificationTypeId);

                if (preference == null)
                {
                    preference = new UserNotificationPreference
                    {
                        UserId = userId,
                        NotificationTypeId = notificationType.NotificationTypeId,
                        IsSubscribed = request.IsSubscribed
                    };
                    await userNotificationPreferenceRepository.Create(preference);
                }
                else
                {
                    preference.IsSubscribed = request.IsSubscribed;
                    await userNotificationPreferenceRepository.Update(preference);
                }

                response.Success = true;
                response.Message = "Thông báo đã được cập nhật thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể cập nhật thông báo. Vui lòng thử lại sau.";
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
