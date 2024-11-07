using Application.Commons;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Commands.CancelPaymentLink
{
    public class CancelPaymentLinkCommandHandler(IPayOSService payOSService) : IRequestHandler<CancelPaymentLinkCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CancelPaymentLinkCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.OrderCode.ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var cancelResult = await payOSService.CancelPaymentLink(request.OrderCode, request.Reason);

                response.Success = true;
                response.Data = cancelResult.Status;
                response.StatusCode = 200;
                response.Message = "Hủy liên kết thanh toán thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể hủy liên kết thanh toán.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
