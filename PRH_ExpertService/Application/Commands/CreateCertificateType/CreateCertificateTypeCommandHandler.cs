using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;

namespace Application.Commands.CreateCertificateType
{
    public class CreateCertificateTypeCommandHandler(
        ICertificateTypeRepository certificateTypeRepository) : IRequestHandler<CreateCertificateTypeCommand, DetailBaseResponse<string>>
    {
        public async Task<DetailBaseResponse<string>> Handle(CreateCertificateTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new DetailBaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Tên chứng chỉ không được để trống.",
                        Field = "Name"
                    });
                    response.Success = false;
                    response.StatusCode = 400;
                    return response;
                }

                var certificateType = new CertificateType
                {
                    CertificateTypeId = Ulid.NewUlid().ToString(),
                    Name = request.Name,
                    Description = request.Description,
                    IsMandatory = request.IsMandatory
                };

                await certificateTypeRepository.Create(certificateType);

                response.Success = true;
                response.Data = certificateType.CertificateTypeId;
                response.Message = "Tạo loại chứng chỉ thành công.";
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
                response.Message = "Có lỗi xảy ra trong quá trình tạo loại chứng chỉ.";
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
