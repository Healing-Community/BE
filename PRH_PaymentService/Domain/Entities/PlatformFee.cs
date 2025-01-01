namespace Domain.Entities;

public class PlatformFee
{
    public required string PlatformFeeId { get; set; }
    public required string PlatformFeeName { get; set; }
    public required string PlatformFeeDescription { get; set; }
    public required int PlatformFeeValue { get; set; } // Value in percentage (%)
}
