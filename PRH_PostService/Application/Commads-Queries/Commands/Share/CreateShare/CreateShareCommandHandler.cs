using System.Security.Claims;
using Application.Commads_Queries.Commands.CreateShare;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;
using Application.Interfaces.AMQP;
using Domain.Constants;
using Domain.Constants.AMQPMessage.Share;

namespace Application.Commands_Queries.Commands.CreateShare;

public class CreateShareCommandHandler(IShareRepository shareRepository, IHttpContextAccessor accessor, IMessagePublisher messagePublisher) : IRequestHandler<CreateShareCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateShareCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BaseResponse<string>.Unauthorized();
            }

            // Check if the platform is Internal and prevent duplicate shares
            if (request.ShareDto.Platform?.Equals("Internal", StringComparison.OrdinalIgnoreCase) == true)
            {
                var existingShare = await shareRepository.GetByPropertyAsync(s =>
                    s.PostId == request.ShareDto.PostId &&
                    s.UserId == userId &&
                    s.Platform == "Internal");

                if (existingShare != null)
                {
                    return BaseResponse<string>.SuccessReturn(existingShare.ShareId, "Bài viết đã được chia sẻ trước đó");
                }
            }

            // Create and save the new share
            var newShare = new Share
            {
                ShareId = Ulid.NewUlid().ToString(),
                PostId = request.ShareDto.PostId ?? string.Empty,
                Platform = request.ShareDto.Platform ?? "Internal",
                Description = request.ShareDto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                UpdatedAt = DateTime.UtcNow.AddHours(7)
            };

            await shareRepository.Create(newShare);

            // Create and publish the ShareMessage
            var shareMessage = new ShareMessage
            {
                PostId = newShare.PostId,
                UserId = newShare.UserId,
                Platform = newShare.Platform,
                Description = newShare.Description,
                ShareDate = newShare.CreatedAt
            };

            await messagePublisher.PublishAsync(shareMessage, QueueName.ShareQueue, cancellationToken);

            var message = request.ShareDto.Platform?.Equals("Internal", StringComparison.OrdinalIgnoreCase) == true
                ? "Chia sẻ bài viết thành công"
                : "Chia sẻ bài viết lên " + request.ShareDto.Platform + " thành công";

            return BaseResponse<string>.SuccessReturn(newShare.ShareId, message);
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.InternalServerError(ex.Message);
        }
    }
}
