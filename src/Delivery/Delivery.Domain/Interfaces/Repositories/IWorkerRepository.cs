using Delivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delivery.Domain.Interfaces.Repositories
{
    public interface IWorkerRepository
    {
        Task<Worker> GetWorkerByName(string workerName);

        Task<Worker> CreateWorkerAsync(Worker worker);

        Task<Worker> UpdateWorkerAsync(Worker worker);
    }
}
