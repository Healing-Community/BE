using Application.Commons.Request.Report;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;

namespace PRH_NotificationService_API.Consumer
{
    public class ReportServiceConsumer(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository) : IConsumer<ReportRequestCreatedMessage>
    {
        public async Task Consume(ConsumeContext<ReportRequestCreatedMessage> context)
        {
            var reportRequest = context.Message;

            var notificationType = await notificationTypeRepository.GetByNameAsync("Report") ?? throw new Exception("Notification type not found");
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = reportRequest.UserId,
                NotificationTypeId = notificationType.NotificationTypeId,
                Message = $"Người dùng {reportRequest.UserId} đã báo cáo bài viết {reportRequest.PostId} với loại báo cáo {reportRequest.ReportTypeId} và trạng thái {reportRequest.Status}",
                CreatedAt = reportRequest.ReportedDate,
                UpdatedAt = reportRequest.ReportedDate,
                IsRead = false
            };

            await notificationRepository.CreateNotificationsAsync([notification]);
        }
    }   
}
