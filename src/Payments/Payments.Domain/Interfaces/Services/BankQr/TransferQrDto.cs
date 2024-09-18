using Payments.Domain.Enums;
using SharedKernel.Constants;
using System;

namespace Payments.Domain.Interfaces.Services;
public class TransferQrDto
{
    public int ClientCode { get; set; }
    public Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public bool SingleUse { get; set; }
    public SystemModules SystemModules { get; set; }
    public long AccountNumber { get; set; }
    public DateTime ExpirationDate { get; set; }
}
