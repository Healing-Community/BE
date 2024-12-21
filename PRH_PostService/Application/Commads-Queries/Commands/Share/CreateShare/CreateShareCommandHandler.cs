using System.Security.Claims;
using Application.Commads_Queries.Commands.CreateShare;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands_Queries.Commands.CreateShare;

public class CreateShareCommandHandler(IShareRepository shareRepository, IHttpContextAccessor accessor) : IRequestHandler<CreateShareCommand, BaseResponse<string>>
{    public async Task<BaseResponse<string>> Handle(CreateShareCommand request, CancellationToken cancellationToken)
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
                    return BaseResponse<string>.SuccessReturn("Bạn đã chia sẻ bài viết này rồi");
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
