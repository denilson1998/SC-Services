using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Domain.Interfaces.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Infrastructure.Persistence.Repositories
{
    public class BillReporsitoy : IBillRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BillReporsitoy(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Bill> CreateAsync(Bill bill, CancellationToken cancellationToken = default)
        {
            await _dbContext.Bills.AddAsync(bill, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return bill;
        }

        public async Task<Bill> GetByIdAsync(int billId, int organizationId, CancellationToken cancellationToken = default)
        {
            var billFound = await _dbContext.Bills
           .Where(b => b.Id == billId)
           .Where(b => b.OrganizationId == organizationId)
           .FirstOrDefaultAsync(cancellationToken);
            return billFound;
        }

        public async Task<Bill> GetBillByIdIncludePaymentsAndVoucherAsync(int billId, int organizationId, CancellationToken cancellationToken = default)
        {
            var billFound = await _dbContext.Bills
           .Where(b => b.Id == billId)
           .Where(b => b.OrganizationId == organizationId)
           .Include(b => b.QrPayments)
           .Include(b => b.VoucherPayments)
           .FirstOrDefaultAsync(cancellationToken);
            return billFound;
        }

        public async Task<Bill> GetBillByIdIncludeQrPaymentsAndBankQrs(int billId, int organizationId, CancellationToken cancellationToken = default)
        {
            var billFound = await _dbContext.Bills
           .Where(b => b.Id == billId)
           .Where(b => b.OrganizationId == organizationId)
           .Include(b => b.BankQrs)
           .Include(b => b.QrPayments)
           .FirstOrDefaultAsync(cancellationToken);
            return billFound;
        }

        public IQueryable<Bill> Get(int organizationId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Bills
           .Where(b => b.OrganizationId == organizationId)
           .AsNoTracking();
        }

        public async Task<Bill> GetBillIncludeBanckQrAsync(string banckQrId, CancellationToken cancellationToken = default)
        {
            return await _dbContext
            .Bills
            // TODO validar si filtra bien
           .Include(b => b.BankQrs.Where(qr => qr.QrId == banckQrId))
           .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Bill> UpdateBill(Bill bill, CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return bill;
        }

        public IQueryable<Bill> GetQueryable(int organizationId, CancellationToken cancellationToken = default)
        {
            return _dbContext
            .Bills
            .Where(b => b.OrganizationId == organizationId)
            .AsQueryable();
        }

        public async Task<int> GetBillCount(int organizationId, CancellationToken cancellationToken = default)
        {
            return await _dbContext
           .Bills
           .Where(b => b.OrganizationId == organizationId)
           .CountAsync(cancellationToken);
        }
    }
}