using System;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPaymentInfo.GetPaymentInfoForGrpc;

public class GetPaymentInfoGrpcQueryHandler(IPaymentInfoRepository paymentInfoRepository) : IRequestHandler<GetPaymentInfoGrpcQuery, BaseResponse<PaymentInfo>>
{
    public async Task<BaseResponse<PaymentInfo>> Handle(GetPaymentInfoGrpcQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var UserId = request.UserId;
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
