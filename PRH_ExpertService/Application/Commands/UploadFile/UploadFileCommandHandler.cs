using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using MediatR;
using NUlid;

namespace Application.Commands.UploadFile
{
    public class UploadFileCommandHandler(
        IFirebaseStorageService firebaseStorageService,
        ICertificateRepository certificateRepository,
        IExpertProfileRepository expertProfileRepository) : IRequestHandler<UploadFileCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertId);
                if (expertProfile == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy chuyên gia với ID được cung cấp.";
                    response.StatusCode = 404;
                    return response;
                }

                if (request.File == null || request.File.Length == 0)
                {
                    response.Success = false;
                    response.Message = "File không hợp lệ.";
                    response.StatusCode = 400;
                    return response;
                }

                var fileName = $"{Ulid.NewUlid()}_{request.File.FileName}";

                string fileUrl;
                using (var memoryStream = new MemoryStream())
                {
                    await request.File.CopyToAsync(memoryStream, cancellationToken);
                    memoryStream.Position = 0;
                    fileUrl = await firebaseStorageService.UploadFileAsync(memoryStream, fileName);
                }

                var certificate = new Domain.Entities.Certificate
                {
                    CertificateId = Ulid.NewUlid().ToString(),
                    ExpertProfileId = request.ExpertId,
                    CertificateTypeId = "1",
                    FileUrl = fileUrl,
                    IssueDate = DateTime.UtcNow,
                    Status = 1
                };

                await certificateRepository.Create(certificate);

                response.Success = true;
                response.Message = "Tải lên tệp thành công.";
                response.Data = fileUrl;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình xử lý.";
                response.StatusCode = 500;
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
