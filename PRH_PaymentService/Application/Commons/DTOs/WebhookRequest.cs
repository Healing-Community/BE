namespace Application.Commons.DTOs;

public class WebhookRequest
{
    public int Error { get; set; }
    public List<TransactionData> Data { get; set; } = [];
}
