using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetAllCertificates
{
    public class GetAllCertificatesQueryHandler(ICertificateRepository certificateRepository)
        : IRequestHandler<GetAllCertificatesQuery, BaseResponse<IEnumerable<Certificate>>>
    {
        public async Task<BaseResponse<IEnumerable<Certificate>>> Handle(GetAllCertificatesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Certificate>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var certificates = await certificateRepository.GetsAsync();

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
