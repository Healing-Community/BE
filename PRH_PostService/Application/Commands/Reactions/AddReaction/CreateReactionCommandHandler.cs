using Application.Commons;
using Application.Commons.Request;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Reactions.AddReaction
{
    public class CreateReactionCommandHandler(IMessagePublisher messagePublisher, IReactionRepository reactionRepository)
        : IRequestHandler<CreateReactionCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReactionCommand request, CancellationToken cancellationToken)
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
            var reaction = new Reaction
            {
                ReactionId = NewId.NextSequentialGuid(),
                UserId = userGuid,
                PostId = request.ReactionDto.PostId,
                ReactionTypeId = request.ReactionDto.ReactionTypeId
            };
            try
            {
                await reactionRepository.Create(reaction);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Reaction created successfully";

                // Send the Request to the Queue for processing
                var reactionRequestCreatedMessage = new ReactionRequestCreatedMessage
                {
                    ReactionRequestId = NewId.NextSequentialGuid(),
                    UserId = reaction.UserId,
                    PostId = reaction.PostId,
                    ReactionTypeId= reaction.ReactionTypeId
                };
                await messagePublisher.PublishAsync(reactionRequestCreatedMessage, QueueName.ReactionQueue, cancellationToken);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to create reaction";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
