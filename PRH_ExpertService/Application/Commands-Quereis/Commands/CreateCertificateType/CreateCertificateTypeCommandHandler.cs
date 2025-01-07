using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.CreateCertificateType
{
    public class CreateCertificateTypeCommandHandler(
        ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<CreateCertificateTypeCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateCertificateTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Kiểm tra đầu vào
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    response.Errors.Add("Tên chứng chỉ không được để trống.");
                    response.Success = false;
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    return response;
                }

                // Tạo loại chứng chỉ mới
                var certificateType = new CertificateType
                {
                    CertificateTypeId = Ulid.NewUlid().ToString(),
                    Name = request.Name,
                    Description = request.Description,
                    IsMandatory = request.IsMandatory
                };

                await certificateTypeRepository.Create(certificateType);

                // Trả về kết quả thành công
                response.Success = true;
                response.Data = certificateType.CertificateTypeId;
                response.Message = "Tạo loại chứng chỉ thành công.";
                response.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi hệ thống
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Success = false;
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
