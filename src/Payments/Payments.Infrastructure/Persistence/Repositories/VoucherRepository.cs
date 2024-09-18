using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Domain.Interfaces.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Infrastructure.Persistence.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public VoucherRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Voucher> CreateAsync(Voucher voucher, CancellationToken cancellationToken = default)
        {
            await _dbContext.Vouchers.AddAsync(voucher, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return voucher;
        }

        public async Task<Voucher> GetAsync(int? voucherId, CancellationToken cancellationToken = default)
        {
            var voucherFound = await _dbContext
               .Vouchers
               .Where(v => v.Id == voucherId)
               .FirstOrDefaultAsync(cancellationToken);
            return voucherFound;
        }

    }
}