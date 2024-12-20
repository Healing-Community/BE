using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commads_Queries.Commands.CreateShare;

public record CreateShareCommand(ShareDto ShareDto) : IRequest<BaseResponse<string>>;