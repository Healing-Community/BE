using System;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commads_Queries.Commands.Share.DeleteShare;

public class DeleteShareCommandHandler(IShareRepository shareRepository) : IRequestHandler<DeleteShareCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(DeleteShareCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var share = await shareRepository.GetByIdAsync(request.ShareId);
            if (share == null)
            {
                return BaseResponse<string>.NotFound("Không tìm thấy bài viết đã chia sẻ"); 
            }
            await shareRepository.DeleteAsync(share.ShareId);
            return BaseResponse<string>.SuccessReturn("Xóa bài viết chia sẻ thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
