using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UserInformation;


namespace Application.Commands.ReportPosts.AddReport
{
    public class CreateReportCommandHandler(IGrpcHelper grpcHelper, IPostRepository postRepository, IHttpContextAccessor accessor, IMessagePublisher messagePublisher)
        : IRequestHandler<CreateReportCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {


            try
            {
                var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return BaseResponse<string>.Unauthorized();

                var post = await postRepository.GetByIdAsync(request.ReportDto.PostId);
                if (post == null) return BaseResponse<string>.NotFound(message: "Bài viết không tồn tại");

                // Get user info
                var userReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                    "UserService",
                    async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = userId })
                );
                if (userReply == null) return BaseResponse<string>.NotFound(message: "Người dùng không tồn tại");
                // Reported user info
                var reportedUserReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                    "UserService",
                    async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = post.UserId })
                );
                if (reportedUserReply == null) return BaseResponse<string>.NotFound(message: "Người dùng không tồn tại");
                var reportRequestCreatedMessage = new PostReportMessage
                {
                    PostTitle = post.Title,
                    ReportedUserEmail = reportedUserReply.Email,
                    ReportedUserId = post.UserId ?? "Data not found",
                    ReportedUserName = reportedUserReply.UserName,
                    UserEmail = userReply.Email,
                    UserName = userReply.UserName,
                    UserId = userId,
                    PostId = post.PostId ?? "Data not found",
                    ReportTypeEnum = request.ReportDto.ReportTypeEnum
                };
                await messagePublisher.PublishAsync(reportRequestCreatedMessage, QueueName.PostReportQueue, cancellationToken);
                // Send mail to user to confirm report
                await messagePublisher.PublishAsync(new SendMailMessage
                {
                    To = userReply.Email,
                    Subject = "Báo cáo bài viết",
                    Body = EmailBody(post.Title, reportedUserReply.UserName)
                }, QueueName.MailQueue, cancellationToken);
                return BaseResponse<string>.SuccessReturn(message: "Báo cáo của bạn đã được gửi thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.InternalServerError(ex.Message);
            }
        }
        // Make this mail formly in the future
        public string EmailBody(string postTitle, string postUserName)
        {
            return $@"
    <html>
    <body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
        <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
            <div style=""text-align: center;"">
                <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
            </div>
            <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Báo cáo bài viết</h2>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Bạn đã báo cáo bài viết <strong>'{postTitle}'</strong> của người dùng <strong>{postUserName}</strong>. 
            </p>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Báo cáo của bạn đã được gửi thành công và sẽ được xử lý trong thời gian sớm nhất.
            </p>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Cảm ơn bạn đã đóng góp vào việc xây dựng cộng đồng <strong>Healing Community</strong>.
            </p>
            <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
        </div>
    </body>
    </html>
    ";
        }

    }
}
