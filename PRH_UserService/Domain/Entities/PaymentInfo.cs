using System;

namespace Domain.Entities;

public class PaymentInfo
{
    public required string PaymentInfoId { get; set; }
    public required string UserId { get; set; }
    public required string BankName { get; set; }
    public required string BankAccountNumber { get; set; }
    public required string BankAccountName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Property
    public User? User { get; set; } // 1 to 1 relationship
}
