﻿using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.RateExpert
{
    public class RateExpertCommandHandler(
        IAppointmentRepository appointmentRepository,
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<RateExpertCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(RateExpertCommand request, CancellationToken cancellationToken)
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

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Errors.Add("Không thể xác định UserId từ yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    response.Success = false;
                    response.Errors.Add("Không tìm thấy lịch hẹn.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (appointment.UserId != userId)
                {
                    response.Success = false;
                    response.Errors.Add("Người dùng không có quyền đánh giá lịch hẹn này.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (appointment.Status != 3) // 3 = Completed
                {
                    response.Success = false;
                    response.Errors.Add("Chỉ có thể đánh giá sau khi tư vấn hoàn thành.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (request.Rating < 1 || request.Rating > 5)
                {
                    response.Success = false;
                    response.Errors.Add("Đánh giá phải nằm trong khoảng từ 1 đến 5.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Cập nhật đánh giá
                appointment.Rating = request.Rating;
                appointment.Comment = request.Comment;
                appointment.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await appointmentRepository.Update(appointment.AppointmentId, appointment);

                // Tính toán lại AverageRating của ExpertProfile
                var expertProfile = await expertProfileRepository.GetByIdAsync(appointment.ExpertProfileId);
                if (expertProfile != null)
                {
                    var appointments = await appointmentRepository.GetByExpertProfileIdAsync(appointment.ExpertProfileId);
                    var ratedAppointments = appointments.Where(a => a.Rating.HasValue && a.Rating.Value > 0).ToList();
                    if (ratedAppointments.Any())
                    {
                        expertProfile.AverageRating = (decimal)ratedAppointments.Average(a => a.Rating.Value);
                        expertProfile.AverageRating = Math.Round(expertProfile.AverageRating * 2, MidpointRounding.AwayFromZero) / 2;

                        // Tăng điểm rating nếu có nhiều lượt đánh giá tốt
                        var goodRatingsCount = ratedAppointments.Count(a => a.Rating >= 4);
                        if (goodRatingsCount >= 5) // Giả sử 5 là ngưỡng để tăng điểm
                        {
                            expertProfile.AverageRating += 0.1M; // Tăng 0.1 điểm
                        }

                        // Đảm bảo điểm rating nằm trong khoảng từ 1 đến 5
                        expertProfile.AverageRating = Math.Clamp(expertProfile.AverageRating, 1, 5);
                    }
                    else
                    {
                        expertProfile.AverageRating = 0;
                    }
                    await expertProfileRepository.Update(expertProfile.ExpertProfileId, expertProfile);
                }

                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Đánh giá thành công.";
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
