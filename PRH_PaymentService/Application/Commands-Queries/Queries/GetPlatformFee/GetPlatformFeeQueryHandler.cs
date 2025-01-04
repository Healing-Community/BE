using Application.Commons;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.GetPlatformFee;

public class GetPlatformFeeQueryHandler(IPlatformFeeRepository platformFeeRepository) : IRequestHandler<GetPlatformFeeQuery, BaseResponse<PlatformFee>>
{
    public async Task<BaseResponse<PlatformFee>> Handle(GetPlatformFeeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var platformFees = await platformFeeRepository.GetsAsync();
            if (platformFees == null)
            {
                return BaseResponse<PlatformFee>.NotFound("Không tìm thấy thông tin phí.");
            }
            return BaseResponse<PlatformFee>.SuccessReturn(platformFees.First());
        }
        catch (Exception e)
        {
            return BaseResponse<PlatformFee>.InternalServerError(e.Message);
            throw;
        }
    }
}
