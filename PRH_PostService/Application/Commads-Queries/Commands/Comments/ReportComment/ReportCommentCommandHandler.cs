using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserInformation;

namespace Application.Commads_Queries.Commands.Comments.ReportComment;

public class ReportCommentCommandHandler(IGrpcHelper grpcHelper, ICommentRepository commentRepository, IHttpContextAccessor accessor, IMessagePublisher messagePublisher) : IRequestHandler<ReportCommentCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(ReportCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponse<string>.Unauthorized();

            var comment = await commentRepository.GetByIdAsync(request.ReportCommentDto.CommentId);
            if (comment == null) return BaseResponse<string>.NotFound(message: "Bình luận không tồn tại");


            var userReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                   "UserService",
                   async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = userId })
               );

            if (userReply == null) return BaseResponse<string>.NotFound(message: "Người dùng không tồn tại");

            var reportedUserReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                   "UserService",
                   async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = comment.UserId })
               );

            if (reportedUserReply == null) return BaseResponse<string>.NotFound(message: "Người dùng không tồn tại");

            var commentReportMessage = new CommentReportMessage
            {
                UserId = userId,
                Content = comment.Content,
                CommentId = comment.CommentId,
                ReportedUserEmail = reportedUserReply.Email,
                ReportedUserId = comment.UserId ?? "Data not found",
                ReportedUserName = reportedUserReply.UserName,
                UserEmail = userReply.Email,
                UserName = userReply.UserName,
                PostId = comment.PostId ?? "Data not found",
                ReportTypeEnum = request.ReportCommentDto.ReportTypeEnum
            };

            await messagePublisher.PublishAsync(commentReportMessage, QueueName.CommentReportQueue, cancellationToken);
            // Send mail to user to confirm report
            await messagePublisher.PublishAsync(new SendMailMessage
            {
                To = userReply.Email,
                Subject = "Báo cáo bình luận",
                Body = EmailBody(comment.Content, reportedUserReply.UserName ?? "Data not found")
            }, QueueName.MailQueue, cancellationToken);
            //if (!IsSended) return BaseResponse<string>.InternalServerError(message:"Báo cáo không thành công vui lòng thử lại sau");
            return BaseResponse<string>.SuccessReturn(message: "Báo cáo thành công bình luận của bạn đã được gửi");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.InternalServerError(ex.Message);
            throw;
        }
    }
    // Make this mail formly in the future
    public string EmailBody(string commentContent, string reportedUserName)
    {
        return $@"
    <html>
    <body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
        <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
            <div style=""text-align: center;"">
                <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
            </div>
            <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Báo cáo bình luận</h2>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Bạn đã báo cáo bình luận <strong>'{commentContent}'</strong> của người dùng <strong>{reportedUserName}</strong>.
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