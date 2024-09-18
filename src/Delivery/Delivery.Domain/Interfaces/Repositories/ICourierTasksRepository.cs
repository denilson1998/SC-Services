using System;
using Delivery.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Delivery.Domain.Interfaces.Repositories
{
    public interface ICourierTasksRepository
    {
        public Task<CourierTask> GetForFareId(int fareId, CancellationToken cancellationToken = default);
        public Task<CourierTask> CreateAsync(CourierTask courierTask, CancellationToken cancellationToken = default);

        public IQueryable<CourierTask> GetAllCourierTaskQuery(int organizationId);

        public Task<List<CourierTask>> GetAllCourierTaskPaidAt();

        public Task<List<CourierTask>> GetAllCourierTaskByStatus();

        public Task UpdateRangeAsync(CancellationToken cancellationToken = default);

        public List<CourierTask> GetAllCourierTaskModified();

        public Task<int> CountCourierTask(CancellationToken cancellationToken);

        public Task<CourierTask> GetForBillId(int billId, int organizationId,
            CancellationToken cancellationToken = default);

        public Task<CourierTask> UpdateAsync(CourierTask courierTask, CancellationToken cancellationToken = default);
        public Task<CourierTask> GetCourierTask(string externalTaskId, CancellationToken cancellationToken = default);

        public Task<CourierTask> UpdateCourierTask(CourierTask courierTask,
            CancellationToken cancellationToken = default);

        public IQueryable<CourierTask> GetLastUpdatedCourierTasksQuery(DateTime lastUpdated);
    }
}