using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;

namespace Application.Commands.CreateNotificationType
{
    public class CreateNotificationTypeCommandHandler : IRequestHandler<CreateNotificationTypeCommand, BaseResponse<string>>
    {
        private readonly INotificationTypeRepository _notificationTypeRepository;

        public CreateNotificationTypeCommandHandler(INotificationTypeRepository notificationTypeRepository)
        {
            _notificationTypeRepository = notificationTypeRepository;
        }

        public async Task<BaseResponse<string>> Handle(CreateNotificationTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                var notificationType = new NotificationType
                {
                    NotificationTypeId = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description
                };

                await _notificationTypeRepository.Create(notificationType);

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
