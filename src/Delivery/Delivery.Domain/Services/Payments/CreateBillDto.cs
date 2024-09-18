using System;
using SharedKernel.Constants;
namespace Delivery.Domain.Interfaces.Services;
public class CreateBillDto
{
    public int OrganizationId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public bool CreateBankQr { get; set; } = true;
}
