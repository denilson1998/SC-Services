using Delivery.Domain.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Domain.Interfaces.Repositories
{
    public interface IWebhookRepository
    {
        public Task<Webhook> GetWebhookAsync(int organizationId, CancellationToken cancellationToken);
        public Task<Webhook> UpdateWebhook(Webhook webhook, CancellationToken cancellationToken);
        public Task<Webhook> CreateWebhookAsync(Webhook webhook, CancellationToken cancellationToken);
        public Task<List<Webhook>> GetAllWebhooks();
    }
}
