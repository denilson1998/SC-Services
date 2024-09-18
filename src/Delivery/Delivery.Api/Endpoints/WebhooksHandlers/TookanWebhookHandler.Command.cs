using Delivery.Infrastructure.Services.Tookan;

namespace Delivery.Api.Endpoints.WebhooksHandlers;

public class TookanWebhook
{
    public int job_id { get; set; }
    public TookanJobStatus job_status { get; set; }
}