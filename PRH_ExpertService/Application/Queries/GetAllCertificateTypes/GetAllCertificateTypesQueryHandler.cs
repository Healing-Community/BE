using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetAllCertificateTypes
{
    public class GetAllCertificateTypesQueryHandler(
        ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<GetAllCertificateTypesQuery, BaseResponse<IEnumerable<CertificateType>>>
    {
        public async Task<BaseResponse<IEnumerable<CertificateType>>> Handle(GetAllCertificateTypesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<CertificateType>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var types = await certificateTypeRepository.GetsAsync();

                response.Success = true;
                response.Data = types;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách loại chứng chỉ thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách loại chứng chỉ.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}