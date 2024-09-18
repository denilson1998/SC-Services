using Ardalis.ApiEndpoints;
using Dapr;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Domain.Services.Webhook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedKernel;
using SharedKernel.Events;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Api.Endpoints.EventHandlers
{
    public class SendCourierTaskStatusUpdate : EndpointBaseAsync
        .WithRequest<CourierTaskUpdatedEventList>
        .WithActionResult
    {
        private readonly IWebhookRepository _webhookRepository;
        private readonly IWebhooksComunicationService _webhooksCommunicationService;

        public SendCourierTaskStatusUpdate(IWebhookRepository webhookRepository,
            IWebhooksComunicationService webhooksCommunicationService
        )
        {
            _webhookRepository = webhookRepository;
            _webhooksCommunicationService = webhooksCommunicationService;
        }

        [Topic(GlobalConstanst.DAPR_PUBSUB_NAME, "CourierTaskUpdatedEventList")]
        [HttpPost("send-delivery-webhook/")]
        //[Authorize(Policy = "MachinePolicy")]
        [SwaggerOperation(
            Summary = "Send courier task webhook",
            Description = "Send courier task status update to respective organization webhook",
            OperationId = "EventHandlers.SendCourierTaskStatusUpdate",
            Tags = new[] { "TasksEndpoint" })
        ]
        public override async Task<ActionResult> HandleAsync([FromBody] CourierTaskUpdatedEventList command,
            CancellationToken cancellationToken = default)
        {
            Console.WriteLine("courier task status update received");
            var webhookFound =
                await _webhookRepository.GetWebhookAsync(command.OrganizationId, cancellationToken);

            if (webhookFound is null)
            {
                throw new Exception("Webhook not found");
            }
            var body = JsonConvert.SerializeObject(command);
            var webHook = new WebhookCommand
            {
                Url = webhookFound.Url,
                IsDeleted = webhookFound.IsDeleted,
                DeletionDateTime = webhookFound.DeletionDateTime,
                OrganizationId = command.OrganizationId,
                Body = body
            };
            await _webhooksCommunicationService.Send(webHook, webhookFound.Url, cancellationToken);
            return Ok();
        }
    }
}