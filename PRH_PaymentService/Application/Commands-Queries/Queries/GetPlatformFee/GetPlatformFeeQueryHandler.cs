using System;
using Application.Commons;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPlatformFee;

public class GetPlatformFeeQueryHandler(IPlatformFeeRepository platformFeeRepository) : IRequestHandler<GetPlatformFeeQuery, BaseResponse<IEnumerable<PlatformFee>>>
{
    public async Task<BaseResponse<IEnumerable<PlatformFee>>> Handle(GetPlatformFeeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var platformFees = await platformFeeRepository.GetsAsync();
            if (platformFees == null)
            {
                return BaseResponse<IEnumerable<PlatformFee>>.NotFound("Không tìm thấy thông tin phí.");
            }
            return BaseResponse<IEnumerable<PlatformFee>>.SuccessReturn(platformFees);
        }
        catch (System.Exception e)
        {
            return BaseResponse<IEnumerable<PlatformFee>>.InternalServerError(e.Message);
            throw;
        }
    }
}
