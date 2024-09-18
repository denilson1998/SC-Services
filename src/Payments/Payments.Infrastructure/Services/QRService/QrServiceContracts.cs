using System;
using Payments.Domain.Enums;
using SharedKernel.Constants;

namespace Payments.Infrastructure.Services.QRService;

public class GenerateQrStringRequest
{
    public int ClientCode { get; set; }
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public bool SingleUse { get; set; }
    public SystemModules SystemModules { get; set; }
    public long AccountNumber { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Metadata { get; set; }
}
