using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands_Queries.Commands.Webhook.Handlewebhook;

public record HandleWebhookCommand(WebhookRequest WebhookRequest) : IRequest<BaseResponse<string>>;
