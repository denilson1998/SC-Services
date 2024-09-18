using System;
using System.Collections.Generic;
using SharedKernel.Constants;

namespace Delivery.Domain.Interfaces.Services;
public class CreateBillResponse
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public decimal TotalAmount { get; set; }
    public Currency Currency { get; set; }
    public IEnumerable<CreateBillBankQrResponse> BankQrs { get; set; }
}

public class CreateBillBankQrResponse
{
    public int Id { get; set; }
    public string ClientName { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public int BankAccountNumber { get; set; }
    public Currency Currency { get; set; }
    public BankAccountType BankAccountType { get; set; }
    public string EncryptedQrString { get; set; }
    public DateTime ExpirationDate { get; set; }
}