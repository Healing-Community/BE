using Application.Commons;
using Domain.Entities;
using MediatR;

public record GetUserReactionByPostIdQuery(string PostId) : IRequest<BaseResponse<Reaction>>;