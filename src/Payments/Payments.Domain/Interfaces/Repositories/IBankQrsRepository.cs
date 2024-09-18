using Payments.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Domain.Interfaces.Repositories
{
    public interface IBankQrsRepository
    {
        public Task<BankQr> CreateAsync(BankQr bankQr, CancellationToken cancellationToken = default);
        public Task<BankQr> GetByIdIncludePaymentsAsync(string bankQrId, CancellationToken cancellationToken = default);

    }
}