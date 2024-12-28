using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.Repositories;
using Domain.Contracts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Queries.GetTransactionHistory
{
    public class GetTransactionHistoryQueryHandler(
        IPaymentRepository paymentRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetTransactionHistoryQuery, BaseResponse<IEnumerable<Payment>>>
    {
        public async Task<BaseResponse<IEnumerable<Payment>>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Payment>>
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

                var payments = await paymentRepository.GetPaymentsByUserIdAsync(userId);
                response.Success = true;
                response.Data = payments;
                response.StatusCode = 200;
                response.Message = "Lấy lịch sử giao dịch thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể lấy lịch sử giao dịch.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
