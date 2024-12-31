﻿using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Queries.Share.CountShareByPostId
{
    public record CountShareByPostIdQuery(PostIdOnlyDto PostId) : IRequest<BaseResponse<ShareCountDto>>;
}