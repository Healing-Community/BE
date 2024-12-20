using System;
using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commads_Queries.Commands.CreateShare;

public class CreateShareCommandHandler(IShareRepository shareRepository, IHttpContextAccessor accessor) : IRequestHandler<CreateShareCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateShareCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var postIndb = await shareRepository.GetByPropertyAsync(s => s.PostId == request.ShareDto.PostId && s.UserId == userId);
            if (request.ShareDto.Platform == "Internal" && postIndb?.Platform == "Internal")
            {
                return BaseResponse<string>.BadRequest("Bạn đã chia sẻ bài viết này rồi");
            }
            var share = new Share
            {
                ShareId = Ulid.NewUlid().ToString(),
                PostId = request.ShareDto.PostId ?? "",
                Platform = request.ShareDto.Platform ?? "Internal",
                Description = request.ShareDto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7)
            };
            await shareRepository.Create(share);
            return BaseResponse<string>.SuccessReturn(share.ShareId);
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
