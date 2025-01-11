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
    public class CreateReportCommandHandler(IGrpcHelper grpcHelper,IPostRepository postRepository, IHttpContextAccessor accessor, IMessagePublisher messagePublisher)
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
                await messagePublisher.PublishAsync(new SendMailMessage{
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
            return $"Bạn đã báo cáo bài viết '{postTitle}' của người dùng {postUserName}. Báo cáo của bạn đã được gửi thành công và sẽ được xử lý trong thời gian sớm nhất. Cảm ơn bạn đã đóng góp vào việc xây dựng cộng đồng";
        }
    }
}
