using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.ApproveCertificate
{
    public class ApproveCertificateCommandHandler(
        ICertificateRepository certificateRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<ApproveCertificateCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(ApproveCertificateCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<ErrorDetail>()
            };

            // Xử lý lỗi hệ thống liên quan đến context
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                response.Success = false;
                response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }

            var userId = Authentication.GetUserIdFromHttpContext(httpContext);
            if (string.IsNullOrEmpty(userId))
            {
                response.Success = false;
                response.Message = "Không thể xác định UserId từ yêu cầu.";
                response.StatusCode = StatusCodes.Status401Unauthorized;
                return response;
            }

            // Xử lý lỗi liên quan đến dữ liệu đầu vào từ người dùng
            var certificate = await certificateRepository.GetByIdAsync(request.CertificateId);
            if (certificate == null)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = $"Chứng chỉ với ID '{request.CertificateId}' không tồn tại.",
                    Field = "CertificateId"
                });
                response.Success = false;
                response.Message = "Có lỗi trong quá trình xử lý yêu cầu.";
                response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return response;
            }

            try
            {
                certificate.Status = 1; // Verified
                certificate.VerifiedByAdminId = userId;
                certificate.VerifiedAt = DateTime.UtcNow.AddHours(7);
                certificate.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await certificateRepository.Update(certificate.CertificateId, certificate);

                response.Success = true;
                response.Data = true;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Chứng chỉ đã được phê duyệt thành công.";
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống khi xử lý logic
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi phê duyệt chứng chỉ.";
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Errors.Add(new ErrorDetail
                {
                    Message = $"Chi tiết lỗi: {ex.Message}",
                    Field = "Exception"
                });
            }

            return response;
        }
    }
}
