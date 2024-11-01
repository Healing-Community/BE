using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetCertificate
{
    public class GetCertificateQueryHandler(ICertificateRepository certificateRepository)
        : IRequestHandler<GetCertificateQuery, BaseResponse<Certificate>>
    {
        public async Task<BaseResponse<Certificate>> Handle(GetCertificateQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Certificate>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var certificate = await certificateRepository.GetByIdAsync(request.CertificateId);
                if (certificate == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy chứng chỉ.";
                    response.StatusCode = 404;
                    return response;
                }

                response.Success = true;
                response.Data = certificate;
                response.StatusCode = 200;
                response.Message = "Lấy thông tin chứng chỉ thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy thông tin chứng chỉ.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
