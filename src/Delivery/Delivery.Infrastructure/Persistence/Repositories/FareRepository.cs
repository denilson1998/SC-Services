using Delivery.Domain.Entities;
using Delivery.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Persistence.Repositories
{
    public class FareRepository : IFareRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FareRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Fare> CreateAsync(Fare fare, CancellationToken cancellationToken = default)
        {
            await _dbContext.Fares.AddAsync(fare, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return fare;
        }

        public async Task<Fare> GetForId(int fareId, CancellationToken cancellationToken = default)
        {
           return await _dbContext.Fares
                 .Where(f => f.Id == fareId)
                 .FirstOrDefaultAsync(cancellationToken);
        }
    }
}