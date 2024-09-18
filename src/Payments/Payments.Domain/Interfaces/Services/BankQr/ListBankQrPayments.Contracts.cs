using System;
using System.Collections.Generic;
using SharedKernel.Constants;

namespace Payments.Domain.Interfaces.Services;

public class ListBankQrPaymentsRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 0;
    public int PerPage { get; set; } = 10;
}


public class ListBankQrPaymentsPagedResult
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PerPage { get; set; }
    public List<ListBankQrPaymentsResult> Data { get; set; } = new List<ListBankQrPaymentsResult>();
}

public class ListBankQrPaymentsResult
{
    public string QrPayId { get; set; } = string.Empty;
    public string QrId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Voucher { get; set; } = string.Empty;
    public string BankOriginName { get; set; } = string.Empty;
    public double Amount { get; set; }
    public Currency Currency { get; set; }
    public DateTime PaymentDate { get; set; }
}
