using Payments.Domain.Enums;
using SharedKernel.Constants;

namespace Payments.Domain.Dtos;
public class BankConfig
{
    public int ClientId { get; set; }
    public int BankAccountNumber { get; set; }
    public SystemModules BankAccountType { get; set; }
}
