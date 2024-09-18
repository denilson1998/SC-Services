using Delivery.Domain.Interfaces.Repositories;
using Delivery.Domain.Webhooks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.Persistence.Repositories
{
    public class WebhookRepository : IWebhookRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public WebhookRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Webhook> GetWebhookAsync(int organizationId, CancellationToken cancellationToken)
        {
            return await _dbContext.Webhooks
                    .IgnoreQueryFilters()
                    .Where(w => w.OrganizationId == organizationId)
                    .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Webhook> CreateWebhookAsync(Webhook webhook, CancellationToken cancellationToken)
        {
            await _dbContext.Webhooks.AddAsync(webhook);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return webhook;
        }

        public async Task<Webhook> UpdateWebhook(Webhook webhook, CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return webhook;
        }

        public async Task<List<Webhook>> GetAllWebhooks()
        {
            return await _dbContext.Webhooks
                .IgnoreQueryFilters()
                .ToListAsync();
        }
    }
}
