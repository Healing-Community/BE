using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace Application.Commands.ReportPosts.AddReport
{
    public class CreateReportCommandHandler(IPostRepository postRepository,IHttpContextAccessor accessor, IMessagePublisher messagePublisher)
        : IRequestHandler<CreateReportCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {

            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BaseResponse<string>.Unauthorized();

            try
            {
                var post = await postRepository.GetByIdAsync(request.ReportDto.PostId);
                if (post == null) return BaseResponse<string>.NotFound(message: "Bài viết không tồn tại");
                
                var reportRequestCreatedMessage = new PostReportMessage
                {
                    UserId = userId,
                    PostId = post.PostId ?? "Data not found",
                    ReportTypeEnum = request.ReportDto.ReportTypeEnum
                };
                await messagePublisher.PublishAsync(reportRequestCreatedMessage, QueueName.PostReportQueue, cancellationToken);
                return BaseResponse<string>.SuccessReturn(message: "Báo cáo của bạn đã được gửi thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.InternalServerError(ex.Message);
            }
        }
    }
}
