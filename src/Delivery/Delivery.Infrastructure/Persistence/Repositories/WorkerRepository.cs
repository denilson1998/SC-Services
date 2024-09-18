using Delivery.Domain.Entities;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Domain.Webhooks;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Persistence.Repositories
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WorkerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Worker> CreateWorkerAsync(Worker worker)
        {
            await _dbContext.Workers.AddAsync(worker);
            await _dbContext.SaveChangesAsync();
            return worker;
        }

        public async Task<Worker> GetWorkerByName(string workerName)
        {
            return await _dbContext
            .Workers
            .Where(w => w.Name == workerName).FirstOrDefaultAsync();
        }

        public async Task<Worker> UpdateWorkerAsync(Worker worker)
        {
            await _dbContext.SaveChangesAsync();
            return worker;
        }
    }
}
