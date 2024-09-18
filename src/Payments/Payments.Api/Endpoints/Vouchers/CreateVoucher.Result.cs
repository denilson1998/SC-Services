using System;
using System.Collections.Generic;
using SharedKernel.Constants;

namespace Payments.Api.Endpoints.Vouchers;

public class CreateVoucherResult
{
    public decimal Value { get; protected set; }
    public Currency Currency { get; protected set; }
    public bool IsPercentage { get; protected set; }
    public int Quantity { get; protected set; }
    public DateTime ValidSince { get; set; }
    public DateTime ValidUntil { get; set; }
}
