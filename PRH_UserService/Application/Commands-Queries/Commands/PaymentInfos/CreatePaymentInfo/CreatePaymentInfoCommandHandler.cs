using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands_Queries.Commands.PaymentInfos.CreatePaymentInfo;
public class CreatePaymentInfoCommandHandler(IPaymentInfoRepository paymentInfoRepository,IHttpContextAccessor accessor) : IRequestHandler<CreatePaymentInfoCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreatePaymentInfoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponse<string>.Unauthorized();
            var paymentInfoExist = await paymentInfoRepository.GetByPropertyAsync(x => x.UserId == userId);
            if (paymentInfoExist != null) return BaseResponse<string>.BadRequest("Bạn đã có thông tin thanh toán");
            var paymentInfo = new PaymentInfo
            {
                PaymentInfoId = Ulid.NewUlid().ToString(),
                BankName = request.PaymentInfoDto.BankName,
                BankAccountName = request.PaymentInfoDto.BankAccountName,
                BankAccountNumber = request.PaymentInfoDto.BankAccountNumber,
                UserId = userId,
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7), // GMT +7
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7) // GMT +7
            };
            await paymentInfoRepository.Create(paymentInfo);
            return BaseResponse<string>.SuccessReturn(paymentInfo.PaymentInfoId,"Tạo thông tin thanh toán thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
