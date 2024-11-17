// using System;
// using System.Security.Claims;
// using Application.Commons;
// using Application.Commons.DTOs;
// using Application.Interfaces.Repository;
// using AutoMapper;
// using Domain.Entities;
// using MassTransit;
// using MediatR;
// using Microsoft.AspNetCore.Http;
// using NUlid;

// namespace Application.Commands_Queries.Commands.Users.UpdateUserProfile.UpdateSocialMediaLink;

// public class UpdateUserSocialMediaLinkCommandHandler(ISocialLinkRepository socialLinkRepository, IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateUserSocialMediaLinkCommand, BaseResponse<string>>
// {
//     public async Task<BaseResponse<string>> Handle(UpdateUserSocialMediaLinkCommand request, CancellationToken cancellationToken)
//     {
//         try
//         {
//             var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
//             if (userId == null)
//             {
//                 return BaseResponse<string>.Unauthorized();
//             }
//             foreach (var item in request.SocialLinkDtos)
//             {
//                 var link = await socialLinkRepository.GetByPropertyAsync(u => u.PlatformName == item.PlatformName);
//                 if (item.PlatformName == link?.PlatformName)
//                 {
//                     link.Url = item.Url;
//                     await socialLinkRepository.UpdateAsync(link.LinkId, link);
//                 }
//                 else
//                 {
//                     var socialLink = new SocialLink
//                     {
//                         LinkId = Ulid.NewUlid().ToString(),
//                         PlatformName = item.PlatformName,
//                         Url = item.Url,
//                         UserId = userId
//                     };
//                     await socialLinkRepository.Create(socialLink);
//                 }
//             }
//         }
//         catch (Exception e)
//         {
//             return BaseResponse<string>.InternalServerError(e.Message);
//         }

//         return BaseResponse<string>.SuccessReturn("Cập nhât thành công");
//     }
// }
