using System;
using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commads_Queries.Commands.Share.UpdateShare;

public record UpdateShareCommand(EditShareDto EditShareDto) : IRequest<BaseResponse<string>>;