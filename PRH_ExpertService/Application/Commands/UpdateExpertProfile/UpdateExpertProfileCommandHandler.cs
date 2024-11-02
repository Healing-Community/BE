using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;
using Domain.Entities;
using Application.Commons.Tools;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.UpdateExpertProfile
{
    public class UpdateExpertProfileCommandHandler(
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateExpertProfileCommand, DetailBaseResponse<bool>>
    {
        public async Task<DetailBaseResponse<bool>> Handle(UpdateExpertProfileCommand request, CancellationToken cancellationToken)
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

                var expertProfile = await expertProfileRepository.GetByIdAsync(request.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Không tìm thấy hồ sơ chuyên gia.",
                        Field = "ExpertProfileId"
                    });
                    response.Success = false;
                    response.StatusCode = 404;
                    return response;
                }

                expertProfile.Specialization = request.Specialization ?? expertProfile.Specialization;
                expertProfile.ExpertiseAreas = request.ExpertiseAreas ?? expertProfile.ExpertiseAreas;
                expertProfile.Bio = request.Bio ?? expertProfile.Bio;
                expertProfile.Status = request.Status ?? expertProfile.Status;
                expertProfile.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await expertProfileRepository.Update(expertProfile.ExpertProfileId, expertProfile);

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
                response.Message = "Cập nhật hồ sơ chuyên gia thành công.";
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = ex.Message,
                    Field = "Exception"
                });
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi cập nhật hồ sơ chuyên gia.";
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
