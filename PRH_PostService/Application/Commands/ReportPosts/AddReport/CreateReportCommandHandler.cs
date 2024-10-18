using Application.Commons;
using Application.Commons.Request;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Entities;
using MassTransit;
using MediatR;
using System.Net;


namespace Application.Commands.ReportPosts.AddReport
{
    public class CreateReportCommandHandler(IMessagePublisher messagePublisher, IReportRepository reportRepository)
        : IRequestHandler<CreateReportCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
            if (userId == null)
            {
                response.Success = false;
                response.Message = "Unauthorized";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return response;
            }
            var userGuid = Guid.Parse(userId);
            var report = new Report
            {
                ReportId = NewId.NextSequentialGuid(),
                PostId = request.reportDto.PostId,
                UserId = userGuid,
                ReportTypeId = request.reportDto.ReportTypeId,
                Status = request.reportDto.Status,
                CreatedAt = DateTime.UtcNow,
            };
            try
            {
                await reportRepository.Create(report);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Report created successfully";
                // Send the Request to the Queue for processing
                var reportRequestCreatedMessage = new ReportRequestCreatedMessage
                {
                    ReportRequestId = NewId.NextSequentialGuid(),
                    PostId = report.PostId,
                    UserId = report.UserId,
                    ReportTypeId = report.ReportTypeId,
                    Status = report.Status,
                    ReportedDate = report.CreatedAt
                };
                await messagePublisher.PublishAsync(reportRequestCreatedMessage, QueueName.ReportQueue, cancellationToken);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to create report";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
