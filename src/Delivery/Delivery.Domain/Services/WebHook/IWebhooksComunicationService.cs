using System;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Domain.Services.Webhook
{
    public interface IWebhooksComunicationService
    {
        public Task<dynamic> Send(WebhookCommand webhook, string url, CancellationToken cancellationToken);
    }

    public class WebhookCommand
    {
        public int OrganizationId { get; set; }
        public string Url { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionDateTime { get; set; }
        public string Body { get; set; }
    }
}