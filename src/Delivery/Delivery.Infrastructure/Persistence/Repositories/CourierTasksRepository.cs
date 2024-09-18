using Delivery.Domain.Entities;
using Delivery.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Persistence.Repositories
{
    public class CourierTasksRepository : ICourierTasksRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMultiTenantService _multiTenantService;

        public CourierTasksRepository(ApplicationDbContext dbContext, IMultiTenantService multiTenantService)
        {
            _dbContext = dbContext;
            _multiTenantService = multiTenantService;
        }

        public async Task<CourierTask> CreateAsync(CourierTask courierTask,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.CourierTasks.AddAsync(courierTask, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return courierTask;
        }

        public async Task<CourierTask> GetForBillId(int billId, int organizationId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext
                .CourierTasks
                .IgnoreQueryFilters()
                .Where(c => c.BillId == billId && c.OrganizationId == organizationId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<CourierTask> GetForFareId(int fareId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.CourierTasks
                .Where(ct => ct.Fare.Id == fareId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<CourierTask> UpdateAsync(CourierTask courierTask,
            CancellationToken cancellationToken = default)
        {
            _multiTenantService.OverrideOrganizationId(courierTask.OrganizationId);
            await _dbContext
                .SaveChangesAsync(cancellationToken);
            return courierTask;
        }

        public async Task UpdateRangeAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<CourierTask> GetAllCourierTaskQuery(int organizationId)
        {
            return _dbContext.CourierTasks
                .Where(task => task.OrganizationId == organizationId)
                .AsNoTracking();
        }

        public async Task<List<CourierTask>> GetAllCourierTaskPaidAt()
        {
            return await _dbContext.CourierTasks
                .IgnoreQueryFilters()
                .Where(c => c.PaidAt == null)
                .ToListAsync();
        }

        public async Task<List<CourierTask>> GetAllCourierTaskByStatus()
        {
            return await _dbContext.CourierTasks
                .IgnoreQueryFilters()
                .Where(c => c.SucceededAt == null && c.FailedAt == null && c.CanceledAt == null)
                .ToListAsync();
        }

        public List<CourierTask> GetAllCourierTaskModified()
        {
            return _dbContext.ChangeTracker
                    .Entries<CourierTask>()
                    .Where(x => x.State == EntityState.Modified)
                    .Select(x => x.Entity)
                    .ToList();
        }

        public Task<int> CountCourierTask(CancellationToken cancellationToken)
        {
            return _dbContext.CourierTasks.CountAsync(cancellationToken);
        }

        public async Task<CourierTask> GetCourierTask(string externalTaskId, CancellationToken cancellationToken)
        {
            return await _dbContext.CourierTasks
                .Where(c => c.ExternalTaskId == externalTaskId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<CourierTask> UpdateCourierTask(CourierTask courierTask, CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return courierTask;
        }

        public IQueryable<CourierTask> GetLastUpdatedCourierTasksQuery(DateTime lastUpdated)
        {
            return _dbContext.CourierTasks
                .IgnoreQueryFilters()
                .Where(task => task.LastStateChangeAt > lastUpdated)
                .AsNoTracking();
        }
    }
}