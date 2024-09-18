using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Infrastructure.Persistence.Repositories
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
            return await _dbContext.Workers
                    .Where(w => w.Name == workerName).FirstOrDefaultAsync();
        }

        public async Task<Worker> UpdateWorkerAsync(Worker worker)
        {
            await _dbContext.SaveChangesAsync();
            return worker;
        }
    }
}
