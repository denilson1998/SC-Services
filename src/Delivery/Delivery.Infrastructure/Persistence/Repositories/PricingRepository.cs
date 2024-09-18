using Delivery.Domain.Entities;
using Delivery.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Persistence.Repositories
{
    public class PricingRepository : IPricingRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PricingRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Pricing> CreateAsync(Pricing pricing, CancellationToken cancellationToken = default)
        {
            await _dbContext.Pricings.AddAsync(pricing, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return pricing;
        }

        public async Task<Pricing> GetForStateIsActive(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Pricings
           .Where(p => p.IsActive)
           .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Pricing> GetForId(int pricingId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Pricings
              .Where(p => p.Id == pricingId)
              .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Pricing>> GetForStateActivePricing(bool isActive, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Pricings
            .Where(p => p.IsActive == isActive)
            .ToListAsync(cancellationToken);
        }

        public async Task<Pricing> UpdateAsync(Pricing pricing, CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return pricing;
        }

        public IQueryable<Pricing> GetAllPricingQuery()
        {
            return  _dbContext
             .Pricings
             .AsNoTracking();
        }

        public int CountPricing(CancellationToken cancellationToken)
        {
            var pricings = _dbContext.Pricings.CountAsync(cancellationToken);
            return Convert.ToInt32(pricings);
        }
    }
}