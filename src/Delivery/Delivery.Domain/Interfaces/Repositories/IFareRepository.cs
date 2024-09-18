using Delivery.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Domain.Interfaces.Repositories
{
    public interface IFareRepository
    {
        public Task<Fare> CreateAsync(Fare fare, CancellationToken cancellationToken = default);
        public Task<Fare> GetForId(int fareId, CancellationToken cancellationToken = default);

    }
}