using Payments.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Domain.Interfaces.Repositories
{
    public interface IVoucherRepository
    {
        public Task<Voucher> CreateAsync(Voucher voucher, CancellationToken cancellationToken = default);

        public Task<Voucher> GetAsync(int? voucherId, CancellationToken cancellationToken = default);
    }
}