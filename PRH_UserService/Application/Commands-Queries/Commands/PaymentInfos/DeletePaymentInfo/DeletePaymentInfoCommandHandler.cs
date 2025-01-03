using System;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Commands.PaymentInfos.DeletePaymentInfo;

public class DeletePaymentInfoCommandHandler(IPaymentInfoRepository paymentInfoRepository,IHttpContextAccessor accessor) : IRequestHandler<DeletePaymentInfoCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(DeletePaymentInfoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var paymentInfo = await paymentInfoRepository.GetByPropertyAsync(x => x.UserId == userId);
            if(paymentInfo == null) return BaseResponse<string>.NotFound(message: "Không tìm thấy thông tin thanh toán");
            await paymentInfoRepository.DeleteAsync(paymentInfo.PaymentInfoId);
            return BaseResponse<string>.SuccessReturn("Xóa thông tin thanh toán thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(message: e.Message);
            throw;
        }
    }
}
