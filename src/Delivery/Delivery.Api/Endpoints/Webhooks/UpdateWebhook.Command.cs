using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Delivery.Api.Endpoints.Webhooks;

public class UpdateWebhookCommand
{
    [JsonIgnore] public int OrganizationId { get; set; }
    [Required] public string Url { get; set; }
}