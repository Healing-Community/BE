using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using Domain.Constants.AMQPMessage.Report;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserInformation;

namespace Application.Commads_Queries.Commands.Posts.BanPost_Moderator;

public class BanPostCommandHandler(IGrpcHelper grpcHelper, IHttpContextAccessor accessor, IMessagePublisher messagePublisher, IPostRepository postRepository) : IRequestHandler<BanPostCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(BanPostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BaseResponse<string>.Unauthorized();
            }
            var post = await postRepository.GetByIdAsync(request.PostId);
            if (post == null)
            {
                return BaseResponse<string>.NotFound();
            }
            if (post.Status == (int)PostStatus.Baned)
            {
                return BaseResponse<string>.SuccessReturn(classInstance: "Bài viết đã bị ban trước đó");
            }
            post.Status = (int)PostStatus.Baned;
            post.Description = "[Bài viết đã bị ban do vi phạm qui định nền tảng]";

            if (request.IsApprove)
            {
                await postRepository.Update(post.PostId, post);
            }

            // Tạo message qua report service để đồng bộ dữ liệu

            await messagePublisher.PublishAsync(new SyncBanPostReportMessage
            {
                PostId = post.PostId,
                IsApprove = request.IsApprove
            }, QueueName.SyncPostReportQueue, cancellationToken);


            // Grpc to user service to get user email (moderator)
            var reportedUserReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                    "UserService",
                    async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = userId })
                );

            if (reportedUserReply == null)
            {
                return BaseResponse<string>.NotFound();
            }

            var message = new BanPostMessage
            {
                PostId = post.PostId,
                UserEmail = reportedUserReply.Email,
                UserId = userId,
                UserName = reportedUserReply.UserName,
                PostTitle = post.Title,
                Reason = request.IsApprove ? "Ban bài viết do vi phạm qui định nền tảng" : "Không ban bài viết do không vi phạm qui định nền tảng",
                IsApprove = request.IsApprove
            };
            // Tạo message qua report service admin xem hành động của moderate
            await messagePublisher.PublishAsync(message, QueueName.BanPostQueue, cancellationToken);

            // Send mail to user to noti if their post has been banned
            await messagePublisher.PublishAsync(new SendMailMessage
            {
                To = reportedUserReply.Email,
                Subject = "Báo cáo bài viết",
                Body = EmailBody(post.Title, reportedUserReply.UserName)
            }, QueueName.MailQueue, cancellationToken);
            
            return BaseResponse<string>.SuccessReturn();

        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
    public string EmailBody(string postTitle, string postUserName)
    {
        return $@"
    <html>
    <body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
        <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
            <div style=""text-align: center;"">
                <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
            </div>
            <h2 style=""color: #ff0000; text-align: center; margin-top: 20px;"">Bài viết bị cấm</h2>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Bài viết của bạn: <strong>'{postTitle}'</strong> đã bị cấm vì vi phạm nguyên tắc cộng đồng của <strong>Healing Community</strong>.
            </p>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Quyết định này được đưa ra sau khi xem xét kỹ lưỡng nội dung bài viết. Người dùng liên quan: <strong>{postUserName}</strong>.
            </p>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Nếu bạn có bất kỳ câu hỏi nào hoặc cần thêm thông tin, vui lòng liên hệ đội ngũ hỗ trợ của chúng tôi.
            </p>
            <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
        </div>
    </body>
    </html>
    ";
    }
}
