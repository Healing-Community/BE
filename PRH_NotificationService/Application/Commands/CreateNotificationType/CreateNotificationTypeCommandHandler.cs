using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands.CreateNotificationType
{
    public class CreateNotificationTypeCommandHandler(INotificationTypeRepository notificationTypeRepository) : IRequestHandler<CreateNotificationTypeCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateNotificationTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var notificationType = new NotificationType
                {
                    NotificationTypeId = Ulid.NewUlid().ToString(),
                    Name = request.Name,
                    Description = request.Description
                };

                await notificationTypeRepository.Create(notificationType);

                response.Success = true;
                response.Message = "Loại thông báo đã được tạo thành công.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Không thể tạo loại thông báo.";
                response.Errors.Add(ex.Message);
                response.StatusCode = 500;
            }

            return response;
        }
    }
}
