using Delivery.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Domain.Interfaces.Repositories
{
    public interface IPricingRepository
    {
        public Task<Pricing> CreateAsync(Pricing pricing, CancellationToken cancellationToken = default);

        public Task<List<Pricing>> GetForStateActivePricing(bool isActive, CancellationToken cancellationToken = default);

        public Task<Pricing> GetForStateIsActive(CancellationToken cancellationToken = default);

        public Task<Pricing> GetForId(int pricingId, CancellationToken cancellationToken = default);

        public Task<Pricing> UpdateAsync(Pricing pricing, CancellationToken cancellationToken = default);

        public IQueryable<Pricing> GetAllPricingQuery();

        public int CountPricing(CancellationToken cancellationToken);
    }
}