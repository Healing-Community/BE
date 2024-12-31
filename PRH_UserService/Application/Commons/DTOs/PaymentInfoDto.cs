using System;

namespace Application.Commons.DTOs;

public class PaymentInfoDto
{
    public required string BankName { get; set; }
    public required string BankAccountNumber { get; set; }
    public required string BankAccountName { get; set; }
}
