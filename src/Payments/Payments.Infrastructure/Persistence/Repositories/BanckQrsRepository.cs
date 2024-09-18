using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Domain.Interfaces.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Infrastructure.Persistence.Repositories
{
    public class BanckQrsRepository : IBankQrsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BanckQrsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BankQr> CreateAsync(BankQr bankQr, CancellationToken cancellationToken = default)
        {
            await _dbContext.BankQrs.AddAsync(bankQr, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return bankQr;
        }

        public async Task<BankQr> GetByIdIncludePaymentsAsync(string bankQrId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.BankQrs
                 .Include(b => b.Payments)
                 .Where(b => b.QrId == bankQrId)
                 .FirstOrDefaultAsync(cancellationToken);
        }
    }
}