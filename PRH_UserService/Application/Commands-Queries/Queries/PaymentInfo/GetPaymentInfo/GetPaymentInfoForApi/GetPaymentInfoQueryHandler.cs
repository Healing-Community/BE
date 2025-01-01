using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Queries.GetPaymentInfo;

public class GetPaymentInfoQueryHandler(IPaymentInfoRepository paymentInfoRepository, IHttpContextAccessor accessor) : IRequestHandler<GetPaymentInfoQuery, BaseResponse<PaymentInfo>>
{
    public async Task<BaseResponse<PaymentInfo>> Handle(GetPaymentInfoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var UserId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null) return BaseResponse<PaymentInfo>.Unauthorized();
            var paymentInfo = await paymentInfoRepository.GetByPropertyAsync(x => x.UserId == UserId);
            if (paymentInfo == null) return BaseResponse<PaymentInfo>.NotFound();
            return BaseResponse<PaymentInfo>.SuccessReturn(paymentInfo);
        }
        catch (Exception e)
        {
            return BaseResponse<PaymentInfo>.InternalServerError(message: e.Message);
            throw;
        }
    }
}
