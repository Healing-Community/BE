using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commads_Queries.Commands.Share.UpdateShare;

public class UpdateShareCommandHandler(IShareRepository shareRepository) : IRequestHandler<UpdateShareCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdateShareCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var share = await shareRepository.GetByIdAsync(request.EditShareDto.ShareId);
            if (share == null)
            {
                return BaseResponse<string>.NotFound("Không tìm thấy bài viết đã chia sẻ"); 
            }
            share.Description = request?.EditShareDto?.Description ?? share.Description;
            await shareRepository.Update(share.ShareId,share);
            return BaseResponse<string>.SuccessReturn("Cập nhật bài viết chia sẻ thành công");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}