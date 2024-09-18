using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Infrastructure.Persistence.Repositories
{
    public class PaymentsRepository : IPaymentsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PaymentsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Payment> CreateAsync(Payment payment, CancellationToken cancellationToken = default)
        {
            await _dbContext.Payments.AddAsync(payment, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return payment;
        }

        public async Task<List<QrPayment>> GetQrPaymentsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.QrPayments
                .ToListAsync();
        }

        public async Task UpdateAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}