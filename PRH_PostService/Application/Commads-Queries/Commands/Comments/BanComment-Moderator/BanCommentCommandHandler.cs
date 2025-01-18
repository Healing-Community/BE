using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using Domain.Constants.AMQPMessage.Report;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserInformation;

namespace Application.Commads_Queries.Commands.Comments.BanComment;

public class BanCommentCommandHandler(IMessagePublisher messagePublisher,IPostRepository postRepository, IGrpcHelper grpcHelper, IHttpContextAccessor accessor, ICommentRepository commentRepository) : IRequestHandler<BanCommentCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(BanCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BaseResponse<string>.Unauthorized();
            }
            var comment = await commentRepository.GetByIdAsync(request.CommentId);
            if (comment == null)
            {
                return BaseResponse<string>.NotFound("Không tìm thấy bình luận");
            }
            // Override comment content
            // Bị ban
            if (request.IsApprove == true)
            {
                comment.Content = "[Bình luận này đã bị ban do vi phạm quy định của hệ thống]";
                comment.CoverImgUrl = null;
                await commentRepository.Update(comment.CommentId, comment);
            }
            // Nếu không ban thì không cần update
            // Grpc to user service to get user email (moderator)

            var userInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserInfo.UserInfoClient, UserInfoRequest, UserInfoResponse>(
                    "UserService",
                    async client => await client.GetUserInfoAsync(new UserInfoRequest { UserId = userId })
                );

            if (userInfoReply == null)
            {
                return BaseResponse<string>.NotFound();
            }

            // Send message to user service to notify user that their comment has been banned
            var message = new BanCommentMessage
            {
                CommentId = comment.CommentId,
                UserId = userId,
                UserName = userInfoReply.UserName,
                UserEmail = userInfoReply.Email,
                Content = comment.Content,
                IsApprove = request.IsApprove
            };

            await messagePublisher.PublishAsync(message, QueueName.BanCommentQueue, cancellationToken);

            await messagePublisher.PublishAsync(new SyncBanCommentReportMessage
            {
                CommentId = comment.CommentId,
                IsApprove = request.IsApprove
            }, QueueName.SyncCommentReportQueue, cancellationToken);



            // Send mail to user to noti if their comment has been banned
            var postInDb = await postRepository.GetByIdAsync(comment.PostId);
            if (postInDb == null)
            {
                return BaseResponse<string>.NotFound("Không tìm thấy bài viết");
            }
            
            await messagePublisher.PublishAsync(new SendMailMessage
            {
                To = userInfoReply.Email,
                Subject = "Báo cáo bình luận",
                Body = EmailBody(comment.Content, postInDb.Title)
            }, QueueName.MailQueue, cancellationToken);


            return BaseResponse<string>.SuccessReturn(message: "Kiểm duyệt bình luận thành công với trạng thái: " + (request.IsApprove ? "Đã ban" : "Không ban"));

        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
    public string EmailBody(string commentContent, string postTitle)
{
    return $@"
    <html>
    <body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
        <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
            <div style=""text-align: center;"">
                <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
            </div>
            <h2 style=""color: #ff0000; text-align: center; margin-top: 20px;"">Bình luận bị cấm</h2>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Bình luận của bạn: <strong>'{commentContent}'</strong> đã bị ban trên bài viết: <strong>'{postTitle}'</strong>.
            </p>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Quyết định này được đưa ra sau khi xem xét kỹ lưỡng nội dung bình luận, do vi phạm nguyên tắc cộng đồng của <strong>Healing Community</strong>.
            </p>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Nếu bạn có thắc mắc hoặc muốn biết thêm chi tiết, vui lòng liên hệ đội ngũ hỗ trợ của chúng tôi.
            </p>
            <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
        </div>
    </body>
    </html>
    ";
}

}
