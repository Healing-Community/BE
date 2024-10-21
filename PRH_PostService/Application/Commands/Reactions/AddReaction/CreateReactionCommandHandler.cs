using Application.Commons;
using Application.Commons.Request.Reaction;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Constants;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.Reactions.AddReaction
{
    public class CreateReactionCommandHandler(IMessagePublisher messagePublisher, IReactionRepository reactionRepository)
        : IRequestHandler<CreateReactionCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReactionCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
            if (userId == null)
            {
                response.Success = false;
                response.Message = "Không có quyền để truy cập";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return response;
            }
            var reaction = new Reaction
            {
                ReactionId = Ulid.NewUlid().ToString(),
                UserId = userId,
                PostId = request.ReactionDto.PostId,
                ReactionTypeId = request.ReactionDto.ReactionTypeId,
                CreateAt = DateTime.UtcNow,
            };
            try
            {
                await reactionRepository.Create(reaction);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Tạo thành công";
                // Send the Request to the Queue for processing
                var reactionRequestCreatedMessage = new ReactionRequestCreatedMessage
                {
                    ReactionRequestId = Ulid.NewUlid().ToString(),
                    UserId = reaction.UserId,
                    PostId = reaction.PostId,
                    ReactionTypeId= reaction.ReactionTypeId,
                    ReactionDate = reaction.CreateAt,
                };
                await messagePublisher.PublishAsync(reactionRequestCreatedMessage, QueueName.ReactionQueue, cancellationToken);
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Tạo thất bại";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
