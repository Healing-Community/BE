using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.UpdateCertificateType
{
    public class UpdateCertificateTypeCommandHandler(
        ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<UpdateCertificateTypeCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(UpdateCertificateTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Kiểm tra xem loại chứng chỉ có tồn tại không
                var certificateType = await certificateTypeRepository.GetByIdAsync(request.CertificateTypeId);
                if (certificateType == null)
                {
                    response.Success = false;
                    response.Errors.Add("Loại chứng chỉ không tồn tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
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
                response.Success = false;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
