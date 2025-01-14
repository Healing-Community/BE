using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Commands.Users.UserReportSystem;

public class UserReportSystemCommandHandler(IUserRepository userRepository, IHttpContextAccessor accessor, IMessagePublisher messagePublisher) : IRequestHandler<UserReportSystemCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UserReportSystemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return BaseResponse<string>.Unauthorized();
            }
            var user = await userRepository.GetByIdAsync(userId);
            var message = new UserReportSystemMessage
            {
                Email = user?.Email,
                UserId = user?.UserId,
                UserName = user?.UserName,
                Content = request.Content
            };
            await messagePublisher.PublishAsync(message: message, QueueName.UserReportSystemQueue, cancellationToken);
            var mail = new SendMailMessage
            {
                To = user?.Email ?? "",
                Subject = "Góp ý từ người dùng",
                Body = "Chúng tôi đã nhận được góp ý của bạn, chúng tôi sẽ ghi nhận và cải thiện dịch vụ của mình.\n Xin chân thành cảm ơn bạn đã góp ý."
            };
            await messagePublisher.PublishAsync(message: mail, QueueName.MailQueue, cancellationToken);
            return BaseResponse<string>.SuccessReturn("Báo cáo đã được gửi");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
