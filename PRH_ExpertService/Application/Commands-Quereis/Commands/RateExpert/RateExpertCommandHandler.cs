using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.RateExpert
{
    public class RateExpertCommandHandler(
        IAppointmentRepository appointmentRepository) : IRequestHandler<RateExpertCommand, BaseResponse<bool>>
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
                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
                if (appointment == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy lịch hẹn.";
                    response.StatusCode = 404;
                    return response;
                }

                if (appointment.Status != 3)
                {
                    response.Success = false;
                    response.Message = "Chỉ có thể đánh giá sau khi tư vấn hoàn thành.";
                    response.StatusCode = 400;
                    return response;
                }

                if (request.Rating < 1 || request.Rating > 5)
                {
                    response.Success = false;
                    response.Message = "Đánh giá phải nằm trong khoảng từ 1 đến 5.";
                    response.StatusCode = 400;
                    return response;
                }

                appointment.Rating = request.Rating;
                appointment.Comment = request.Comment;
                await appointmentRepository.Update(appointment.AppointmentId, appointment);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Đánh giá thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi đánh giá.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
