using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetCertificatesByExpert
{
    public class GetCertificatesByExpertQueryHandler(ICertificateRepository certificateRepository)
        : IRequestHandler<GetCertificatesByExpertQuery, BaseResponse<IEnumerable<Certificate>>>
    {
        public async Task<BaseResponse<IEnumerable<Certificate>>> Handle(GetCertificatesByExpertQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Certificate>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                // Lấy danh sách chứng chỉ theo expertProfileId
                var certificates = await certificateRepository.GetByExpertProfileIdAsync(request.ExpertProfileId);

                if (certificates == null || !certificates.Any())
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy chứng chỉ nào cho chuyên gia.";
                    response.StatusCode = 404;
                    return response;
                }

                response.Success = true;
                response.Data = certificates;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách chứng chỉ thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách chứng chỉ.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
