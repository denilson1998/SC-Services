using Payments.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Domain.Interfaces.Repositories;

public interface IBillRepository
{
    public Task<Bill> CreateAsync(Bill bill, CancellationToken cancellationToken = default);

    public Task<Bill> UpdateBill(Bill bill, CancellationToken cancellationToken = default);

    public IQueryable<Bill> Get(int organizationId, CancellationToken cancellationToken = default);

    public Task<Bill> GetByIdAsync(int billId, int organizationId, CancellationToken cancellationToken = default);

    public Task<Bill> GetBillIncludeBanckQrAsync(string banckQrId, CancellationToken cancellationToken = default);

    public Task<Bill> GetBillByIdIncludePaymentsAndVoucherAsync(int billId, int organizationId, CancellationToken cancellationToken = default);

    public Task<Bill> GetBillByIdIncludeQrPaymentsAndBankQrs(int billId, int organizationId, CancellationToken cancellationToken = default);

    public IQueryable<Bill> GetQueryable(int organizationId, CancellationToken cancellationToken = default);

    public Task<int> GetBillCount(int organizationId, CancellationToken cancellationToken = default);
}