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
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateCertificateCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(UpdateCertificateCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<bool>
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
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Lỗi hệ thống: không thể xác định context của yêu cầu.",
                        Field = "HttpContext"
                    });
                    response.Success = false;
                    response.StatusCode = 400;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);

                var certificate = await certificateRepository.GetByIdAsync(request.CertificateId);
                if (certificate == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Chứng chỉ không tồn tại.",
                        Field = "CertificateId"
                    });
                    response.Success = false;
                    response.StatusCode = 404;
                    return response;
                }

                if (!string.IsNullOrEmpty(request.CertificateTypeId))
                {
                    var certificateType = await certificateTypeRepository.GetByIdAsync(request.CertificateTypeId);
                    if (certificateType == null)
                    {
                        response.Errors.Add(new ErrorDetail
                        {
                            Message = "Loại chứng chỉ không hợp lệ.",
                            Field = "CertificateTypeId"
                        });
                        response.Success = false;
                        response.StatusCode = 400;
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
                response.StatusCode = 200;
                response.Message = "Cập nhật chứng chỉ thành công.";
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = ex.Message,
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật chứng chỉ.";
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
