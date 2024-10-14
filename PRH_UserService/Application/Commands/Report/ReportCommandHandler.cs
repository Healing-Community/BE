using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Report
{
    public class ReportCommandHandler(IMessagePublisher messagePublisher, IMapper mapper) : IRequestHandler<ReportCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(ReportCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>() // Initialize the error list
            };
            try
            {
                // Map the ReportMessageDto to ReportMessage
                var reportMessage = mapper.Map<ReportMessage>(request.ReportMessageDto);
                // Get the user id from the context
                var userId = Authentication.GetUserIdFromHttpContext(request.ReportMessageDto.context);
                // Check if the user id is null so the user is unauthorized
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Unauthorized";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }
                // Set the user id to the report message
                reportMessage.UserId = Guid.Parse(userId);
                // Publish the report message to the queue
                await messagePublisher.PublishAsync(reportMessage, QueueName.ReportQueue, cancellationToken);
                // Set the response properties
                response.Success = true;
                response.Message = "Report has been sent successfully";
                response.StatusCode = (int)HttpStatusCode.OK;
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Report has not been sent";
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return response;
            }
        }
    }
}
