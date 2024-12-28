using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.DeleteWorkExperience
{
    public class DeleteWorkExperienceCommandHandler(
        IWorkExperienceRepository workExperienceRepository) : IRequestHandler<DeleteWorkExperienceCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(DeleteWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var workExperience = await workExperienceRepository.GetByIdAsync(request.WorkExperienceId);
                if (workExperience == null)
                {
                    response.Success = false;
                    response.Message = "Kinh nghiệm làm việc không tồn tại.";
                    response.StatusCode = 404;
                    return response;
                }

                await workExperienceRepository.DeleteAsync(request.WorkExperienceId);

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa kinh nghiệm làm việc thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi xóa kinh nghiệm làm việc.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}