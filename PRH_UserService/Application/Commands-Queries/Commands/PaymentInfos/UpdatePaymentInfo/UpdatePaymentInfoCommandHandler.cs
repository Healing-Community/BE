using System;
using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Commands.PaymentInfos.UpdatePaymentInfo;

public class UpdatePaymentInfoCommandHandler(IPaymentInfoRepository paymentInfoRepository,IHttpContextAccessor accessor) : IRequestHandler<UpdatePaymentInfoCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdatePaymentInfoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponse<string>.Unauthorized();
            var paymentInfo = await paymentInfoRepository.GetByPropertyAsync(x => x.UserId == userId);
            if (paymentInfo == null) return BaseResponse<string>.NotFound();
            paymentInfo.BankName = request.PaymentInfoDto.BankName;
            paymentInfo.BankAccountName = request.PaymentInfoDto.BankAccountName;
            paymentInfo.BankAccountNumber = request.PaymentInfoDto.BankAccountNumber;
            paymentInfo.UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7); // GMT +7
            await paymentInfoRepository.UpdateAsync(paymentInfo.PaymentInfoId,paymentInfo);
            return BaseResponse<string>.SuccessReturn(paymentInfo.PaymentInfoId,"Cập nhật thông tin thanh toán thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(message: e.Message);
            throw;
        }
    }
}
