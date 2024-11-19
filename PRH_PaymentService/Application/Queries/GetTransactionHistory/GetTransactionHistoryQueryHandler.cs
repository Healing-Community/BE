using Application.Commons;
using Application.Interfaces.Repositories;
using Domain.Contracts;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetTransactionHistory
{
    public class GetTransactionHistoryQueryHandler(IPaymentRepository paymentRepository) : IRequestHandler<GetTransactionHistoryQuery, BaseResponse<IEnumerable<Payment>>>
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
                var payments = await paymentRepository.GetPaymentsByUserIdAsync(request.UserId);
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
