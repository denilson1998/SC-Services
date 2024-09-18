using System.Threading;
using System.Threading.Tasks;
using Payments.Domain.Dtos;

namespace Payments.Domain.Interfaces.Repositories;

public interface IBankConfigRepository
{
    public Task<BankConfig> GetBankConfigAsync(CancellationToken cancellationToken = default);
}
