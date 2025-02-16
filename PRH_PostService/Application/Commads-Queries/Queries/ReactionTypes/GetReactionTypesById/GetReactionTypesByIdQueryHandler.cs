﻿using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Queries.ReactionTypes.GetReactionTypesById
{
    public class GetReactionTypesByIdQueryHandler(IReactionTypeRepository reactionTypeRepository)
        : IRequestHandler<GetReactionTypesByIdQuery, BaseResponse<ReactionType>>
    {
        public async Task<BaseResponse<ReactionType>> Handle(GetReactionTypesByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<ReactionType>()
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var reactionType = await reactionTypeRepository.GetByIdAsync(request.Id);
                if (reactionType == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy";
                    response.Errors.Add("Không tìm thấy dữ liệu nào có ID được cung cấp.");
                    return response;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = reactionType;
                response.Success = true;
                response.Message = "Lấy dữ liệu thành công";
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Có lỗi xảy ra";
                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
