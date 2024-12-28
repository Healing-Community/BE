using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Microsoft.AspNetCore.Http;
using Application.Commons.Tools;

namespace Application.Commands.DeleteCertificate
{
    public class DeleteCertificateCommandHandler(
        ICertificateRepository certificateRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeleteCertificateCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(DeleteCertificateCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.";
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);

                var certificate = await certificateRepository.GetByIdAsync(request.CertificateId);
                if (certificate == null)
                {
                    response.Success = false;
                    response.Message = "Chứng chỉ không tồn tại.";
                    response.StatusCode = 404;
                    return response;
                }

                await certificateRepository.DeleteAsync(request.CertificateId);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Xóa chứng chỉ thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi xóa chứng chỉ.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
