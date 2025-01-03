using System;
using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands_Queries.Commands.PaymentInfos.UpdatePaymentInfo;

public class UpdatePaymentInfoCommandHandler(IPaymentInfoRepository paymentInfoRepository, IHttpContextAccessor accessor) : IRequestHandler<UpdatePaymentInfoCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdatePaymentInfoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get userId from token
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponse<string>.Unauthorized();

            //Update payment info if exist otherwise create new one

            var paymentInfo = await paymentInfoRepository.GetByPropertyAsync(x => x.UserId == userId);
            if (paymentInfo == null)
            {
                // create new payment info
                var newPaymentInfo = new PaymentInfo
                {
                    PaymentInfoId = Ulid.NewUlid().ToString(),
                    BankName = request.PaymentInfoDto.BankName,
                    BankAccountName = request.PaymentInfoDto.BankAccountName,
                    BankAccountNumber = request.PaymentInfoDto.BankAccountNumber,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7), // GMT +7
                    UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7) // GMT +7
                };
                await paymentInfoRepository.Create(newPaymentInfo);
                return BaseResponse<string>.SuccessReturn(newPaymentInfo.PaymentInfoId, "Tạo thông tin thanh toán thành công");
            }
            else
            {
                // update payment info
                paymentInfo.BankName = request.PaymentInfoDto.BankName;
                paymentInfo.BankAccountName = request.PaymentInfoDto.BankAccountName;
                paymentInfo.BankAccountNumber = request.PaymentInfoDto.BankAccountNumber;
                paymentInfo.UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7); // GMT +7
                await paymentInfoRepository.UpdateAsync(paymentInfo.PaymentInfoId, paymentInfo);
                return BaseResponse<string>.SuccessReturn(paymentInfo.PaymentInfoId, "Cập nhật thông tin thanh toán thành công");
            }

        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(message: e.Message);
            throw;
        }
    }
}
