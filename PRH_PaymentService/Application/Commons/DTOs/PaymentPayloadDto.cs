public class PaymentPayloadDto
{
    public string? AppointmentId { get; set; }
    public long OrderCode { get; set; }
    public int Amount { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}