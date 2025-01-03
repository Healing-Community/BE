using System;

namespace Application.Commons.DTOs;

public class TransactionData
{
    public int Id { get; set; }
    public string Tid { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Amount { get; set; }
    public int CusumBalance { get; set; }
    public string When { get; set; } = string.Empty;
    public string Bank_sub_acc_id { get; set; } = string.Empty;
    public string SubAccId { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BankAbbreviation { get; set; } = string.Empty;
    public string VirtualAccount { get; set; } = string.Empty;
    public string VirtualAccountName { get; set; } = string.Empty;
    public string CorresponsiveName { get; set; } = string.Empty;
    public string CorresponsiveAccount { get; set; } = string.Empty;
    public string CorresponsiveBankId { get; set; } = string.Empty;
    public string CorresponsiveBankName { get; set; } = string.Empty;
}
