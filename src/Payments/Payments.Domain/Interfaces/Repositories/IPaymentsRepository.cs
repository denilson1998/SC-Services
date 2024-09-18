using Payments.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Domain.Interfaces.Repositories
{
    public interface IPaymentsRepository
    {
        public Task<Payment> CreateAsync(Payment payment, CancellationToken cancellationToken = default);

        public Task<List<QrPayment>> GetQrPaymentsAsync(CancellationToken cancellationToken = default);

        public Task UpdateAsync(CancellationToken cancellationToken = default);
    }
}