using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUlid;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentCallbackController(
        IExpertAvailabilityRepository availabilityRepository,
        IAppointmentRepository appointmentRepository,
        IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpPost("payment-result")]
        public async Task<IActionResult> PaymentResult([FromBody] PaymentResultDto paymentResult)
        {
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    return BadRequest("Lỗi hệ thống: không thể xác định context của yêu cầu.");
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext); // Lấy userId từ token
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Không tìm thấy ID người dùng.");
                }

                var availability = await availabilityRepository.GetByIdAsync(paymentResult.ExpertAvailabilityId);
                if (availability == null)
                {
                    return NotFound("Lịch trống không tồn tại.");
                }

                if (paymentResult.Success)
                {
                    var appointment = new Appointment
                    {
                        AppointmentId = Ulid.NewUlid().ToString(),
                        UserId = userId, // Sử dụng userId từ token
                        ExpertProfileId = availability.ExpertProfileId,
                        ExpertAvailabilityId = availability.ExpertAvailabilityId,
                        AppointmentDate = availability.AvailableDate,
                        StartTime = availability.StartTime,
                        EndTime = availability.EndTime,
                        Status = 2, // Đã đặt
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await appointmentRepository.Create(appointment);
                    availability.Status = 2; // Đã đặt
                }
                else
                {
                    availability.Status = 0; // Có sẵn
                }

                availability.UpdatedAt = DateTime.UtcNow;
                await availabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                return Ok("Trạng thái thanh toán và lịch hẹn đã được cập nhật.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi trong quá trình xử lý thanh toán: {ex.Message}");
            }
        }
    }
}
