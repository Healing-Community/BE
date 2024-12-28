using System;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Commands.Share.DeleteShare;

public record DeleteShareCommand(string ShareId) : IRequest<BaseResponse<string>>;