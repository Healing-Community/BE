using Application.Commons;
using Application.Commons.Request;
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
            var reactionId = NewId.NextSequentialGuid();
            var reaction = new Reaction
            {
                ReactionId = reactionId,
                PostId = request.ReactionDto.PostId,
                ReactionTypeId = request.ReactionDto.ReactionTypeId
            };
            var response = new BaseResponse<string>
            {
                Id = reactionId,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                await reactionRepository.Create(reaction);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Reaction created successfully";

                // Send the Request to the Queue for processing
                //var postingRequestCreatedMessage = new PostingRequestCreatedMessage
                //{
                //    PostedDate = post.CreateAt,
                //    PostingRequestId = NewId.NextSequentialGuid(),
                //    Tittle = post.Title,
                //    UserId = post.UserId,
                //};
                //await messagePublisher.PublishAsync(postingRequestCreatedMessage, QueueName.PostQueue, cancellationToken);
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
