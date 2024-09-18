using System;
using System.Collections.Generic;
using SharedKernel.Constants;

namespace Payments.Api.Endpoints.Bills;

public class RegisterBillPaymentResult
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public decimal TotalAmount { get; set; }
    public Currency Currency { get; set; }
    public IEnumerable<CreateBillBankQrResult> BankQrs { get; set; }
}
