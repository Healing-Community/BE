using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Commands.Comments.ReportComment;

public class ReportCommentCommandHandler(ICommentRepository commentRepository, IHttpContextAccessor accessor, IMessagePublisher messagePublisher) : IRequestHandler<ReportCommentCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(ReportCommentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponse<string>.Unauthorized();

            var comment = await commentRepository.GetByIdAsync(request.ReportCommentDto.CommentId);
            if (comment == null) return BaseResponse<string>.NotFound(message:"Bình luận không tồn tại");

            var commentReportMessage = new CommentReportMessage
            {
                UserId = userId,
                Content = comment.Content,
                CommentId = comment.CommentId,
                PostId = comment.PostId ?? "Data not found",
                ReportTypeEnum = request.ReportCommentDto.ReportTypeEnum
            };

            await messagePublisher.PublishAsync(commentReportMessage, QueueName.CommentReportQueue, cancellationToken);
            //if (!IsSended) return BaseResponse<string>.InternalServerError(message:"Báo cáo không thành công vui lòng thử lại sau");
            return BaseResponse<string>.SuccessReturn(message:"Báo cáo thành công bình luận của bạn đã được gửi");
        }
        catch(Exception ex)
        {
            return BaseResponse<string>.InternalServerError(ex.Message);
            throw;
        }
    }
}
