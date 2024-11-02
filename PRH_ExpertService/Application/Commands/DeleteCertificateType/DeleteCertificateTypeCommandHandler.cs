using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.DeleteCertificateType
{
    public class DeleteCertificateTypeCommandHandler(
        ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<DeleteCertificateTypeCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(DeleteCertificateTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var certificateType = await certificateTypeRepository.GetByIdAsync(request.CertificateTypeId);
                if (certificateType == null)
                {
                    response.Success = false;
                    response.Message = "Loại chứng chỉ không tồn tại.";
                    response.StatusCode = 404;
                    return response;
                }

                await certificateTypeRepository.DeleteAsync(request.CertificateTypeId);

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa loại chứng chỉ thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi xóa loại chứng chỉ.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}