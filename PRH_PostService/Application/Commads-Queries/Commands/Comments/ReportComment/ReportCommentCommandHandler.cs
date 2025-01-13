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
        return $"Bạn đã báo cáo bình luận '{commentContent}' của người dùng {reportedUserName}. Báo cáo của bạn đã được gửi thành công và sẽ được xử lý trong thời gian sớm nhất. Cảm ơn bạn đã đóng góp vào việc xây dựng cộng đồng";
    }
}