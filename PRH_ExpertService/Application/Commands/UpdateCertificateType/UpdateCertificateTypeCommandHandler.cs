using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Domain.Entities;
using NUlid;

namespace Application.Commands.UpdateCertificateType
{
    public class UpdateCertificateTypeCommandHandler(
        ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<UpdateCertificateTypeCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(UpdateCertificateTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var certificateType = await certificateTypeRepository.GetByIdAsync(request.CertificateTypeId);
                if (certificateType == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Loại chứng chỉ không tồn tại.",
                        Field = "CertificateTypeId"
                    });
                    response.Success = false;
                    response.StatusCode = 404;
                    return response;
                }

                certificateType.Name = request.Name;
                certificateType.Description = request.Description;
                certificateType.IsMandatory = request.IsMandatory;

                await certificateTypeRepository.Update(request.CertificateTypeId, certificateType);

                response.Success = true;
                response.Data = true;
                response.Message = "Cập nhật loại chứng chỉ thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = ex.Message,
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật loại chứng chỉ.";
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
