using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.UpdateCertificate
{
    public class UpdateCertificateCommandHandler(
        ICertificateRepository certificateRepository,
        ICertificateTypeRepository certificateTypeRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateCertificateCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(UpdateCertificateCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Errors.Add("Lỗi hệ thống: không thể xác định context của yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Errors.Add("Không thể xác định UserId từ yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                var certificate = await certificateRepository.GetByIdAsync(request.CertificateId);
                if (certificate == null)
                {
                    response.Success = false;
                    response.Errors.Add("Chứng chỉ không tồn tại.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (!string.IsNullOrEmpty(request.CertificateTypeId))
                {
                    var certificateType = await certificateTypeRepository.GetByIdAsync(request.CertificateTypeId);
                    if (certificateType == null)
                    {
                        response.Success = false;
                        response.Errors.Add("Loại chứng chỉ không hợp lệ.");
                        response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                        response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                        return response;
                    }
                    certificate.CertificateTypeId = request.CertificateTypeId;
                }

                certificate.IssueDate = request.IssueDate ?? certificate.IssueDate;
                certificate.ExpirationDate = request.ExpirationDate ?? certificate.ExpirationDate;
                certificate.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await certificateRepository.Update(certificate.CertificateId, certificate);

                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Cập nhật chứng chỉ thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
