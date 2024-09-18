using Microsoft.EntityFrameworkCore;
using Payments.Domain;
using Payments.Domain.Dtos;
using Payments.Domain.Enums;
using Payments.Domain.Interfaces.Repositories;
using SharedKernel.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Infrastructure.Persistence.Repositories
{
    public class BankConfigRepository : IBankConfigRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BankConfigRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<BankConfig> GetBankConfigAsync(CancellationToken cancellationToken = default)
        {
            return Task<BankConfig>.FromResult(new BankConfig()
            {
                // ClientId = 74593,
                // BankAccountNumber = 3062861,
                // ClientId = 2425490,
                ClientId = 936476,
                BankAccountNumber = 692316153,
                BankAccountType = SystemModules.SavingAccountModule
            });
        }
    }
}