using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetCertificateType
{
    public class GetCertificateTypeQueryHandler(
        ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<GetCertificateTypeQuery, BaseResponse<CertificateType>>
    {
        public async Task<BaseResponse<CertificateType>> Handle(GetCertificateTypeQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<CertificateType>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var type = await certificateTypeRepository.GetByIdAsync(request.CertificateTypeId);
                if (type == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy loại chứng chỉ.";
                    response.StatusCode = 404;
                    return response;
                }

                response.Success = true;
                response.Data = type;
                response.StatusCode = 200;
                response.Message = "Lấy thông tin loại chứng chỉ thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy thông tin loại chứng chỉ.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}