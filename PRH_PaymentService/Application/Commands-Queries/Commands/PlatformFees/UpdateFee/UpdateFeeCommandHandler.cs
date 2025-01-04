using Application.Commons;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Commands_Queries.Commands.PlatformFees.UpdateFee;

public class UpdateFeeCommandHandler(IPlatformFeeRepository platformFeeRepository) : IRequestHandler<UpdateFeeCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdateFeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var platformFee = await platformFeeRepository.GetByIdAsync(request.PlatformFeeId.ToString());
            if (platformFee == null)
            {
                return BaseResponse<string>.NotFound("Không tìm thấy thông tin phí.");
            }
            platformFee.PlatformFeeValue = request.Percent;
            await platformFeeRepository.Update(platformFee.PlatformFeeId, platformFee);
            return BaseResponse<string>.SuccessReturn("Cập nhật phí thành công.");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
