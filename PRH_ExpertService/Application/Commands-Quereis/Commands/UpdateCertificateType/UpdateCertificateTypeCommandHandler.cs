using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.UpdateCertificateType
{
    public class UpdateCertificateTypeCommandHandler(
        ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<UpdateCertificateTypeCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(UpdateCertificateTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<ErrorDetail>()
            };

            try
            {
                // Kiểm tra xem loại chứng chỉ có tồn tại không
                var certificateType = await certificateTypeRepository.GetByIdAsync(request.CertificateTypeId);
                if (certificateType == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Loại chứng chỉ không tồn tại.",
                        Field = "CertificateTypeId"
                    });
                    response.Success = false;
                    response.Message = "Có lỗi trong dữ liệu đầu vào.";
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                // Cập nhật thông tin loại chứng chỉ
                certificateType.Name = request.Name;
                certificateType.Description = request.Description;
                certificateType.IsMandatory = request.IsMandatory;

                await certificateTypeRepository.Update(request.CertificateTypeId, certificateType);

                // Trả về kết quả thành công
                response.Success = true;
                response.Data = true;
                response.Message = "Cập nhật loại chứng chỉ thành công.";
                response.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi hệ thống
                response.Errors.Add(new ErrorDetail
                {
                    Message = ex.Message,
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật loại chứng chỉ.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
