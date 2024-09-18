using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.RouteAndBodyModelBinding;
using Delivery.Domain.Webhooks;
using Delivery.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Delivery.Api.Endpoints.Webhooks;

public class Create : EndpointBaseAsync
    .WithRequest<UpdateWebhookCommand>
    .WithActionResult
{
    private readonly IWebhookRepository _webhookRepository;

    public Create(
        IWebhookRepository webhookRepository
    )
    {
        _webhookRepository = webhookRepository;
    }

    [HttpPost("organizations/{OrganizationId}/webhooks/")]
    [Authorize(Policy = "UserM2MPolicy")]
    [SwaggerOperation(
        Summary = "Create or update Webhook",
        Description = "Create or update Webhook",
        OperationId = "Webhooks.CreateWebhook",
        Tags = new[] { "WebhooksEndpoint" })
    ]
    public override async Task<ActionResult> HandleAsync([FromRouteAndBody] UpdateWebhookCommand command,
        CancellationToken cancellationToken = default)
    {
        var webhookFound = await _webhookRepository.GetWebhookAsync(command.OrganizationId, cancellationToken);

        bool isNull = webhookFound is null ? true : false;

        webhookFound ??= new Webhook()
        {
            OrganizationId = command.OrganizationId,
        };

        webhookFound.Url = command.Url;

        _ = isNull ? await _webhookRepository.CreateWebhookAsync(webhookFound, cancellationToken) : await _webhookRepository.UpdateWebhook(webhookFound, cancellationToken);

        return Ok();
    }
}