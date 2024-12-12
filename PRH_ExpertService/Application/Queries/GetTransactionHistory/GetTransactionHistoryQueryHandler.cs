using Application.Commons;
using Application.Commons.DTOs;
using Application.Commons.Tools;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Queries.GetTransactionHistory
{
    public class GetTransactionHistoryQueryHandler(
        IAppointmentRepository appointmentRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetTransactionHistoryQuery, BaseResponse<ICollection<TransactionHistoryDTO>>>
    {
        public async Task<BaseResponse<ICollection<TransactionHistoryDTO>>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<ICollection<TransactionHistoryDTO>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy UserId từ token trong HttpContext
                var userId = Authentication.GetUserIdFromHttpContext(httpContextAccessor.HttpContext);
                Console.WriteLine($"UserId retrieved from token: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("UserId is null or empty. Unauthorized request.");
                    response.Success = false;
                    response.Message = "Không thể xác thực người dùng.";
                    response.StatusCode = 401;
                    return response;
                }

                // Truy vấn lịch sử giao dịch từ UserId
                var appointments = await appointmentRepository.GetByUserIdAsync(userId);
                Console.WriteLine($"Number of appointments retrieved: {appointments.Count()}");

                // Ánh xạ dữ liệu sang DTO
                var transactionHistory = appointments.Select(a => new TransactionHistoryDTO
                {
                    ExpertName = a.ExpertProfile.Fullname,
                    ExpertEmail = a.ExpertEmail,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                }).ToList();
                Console.WriteLine($"Transaction history mapped to DTO: {transactionHistory.Count} records");

                response.Success = true;
                response.Data = transactionHistory;
                response.StatusCode = 200;
                response.Message = "Lấy lịch sử giao dịch thành công.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy lịch sử giao dịch.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
