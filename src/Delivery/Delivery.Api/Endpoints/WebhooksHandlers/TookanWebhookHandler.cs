using Ardalis.ApiEndpoints;
using Delivery.Domain.Entities;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Infrastructure.Services.Tookan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Events;
using Sitec.Delivery.EventBus.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Api.Endpoints.WebhooksHandlers;

public class TookanWebhookHandler : EndpointBaseAsync
    .WithRequest<TookanWebhook>
    .WithActionResult
{
    private readonly ICourierTasksRepository _courierTaskRepository;

    private readonly IEventBus _eventBus;

    public TookanWebhookHandler(
        ICourierTasksRepository courierTasksRepository,
        IEventBus eventBus
    )
    {
        _courierTaskRepository = courierTasksRepository;
        _eventBus = eventBus;
    }

    [HttpPost("tookan-webhook-handler/")]
    //[Authorize(Policy = "MachinePolicy")]
    [SwaggerOperation(
        Summary = "Handles tookan webhook",
        Description = "Handles tookan webhook",
        OperationId = "WebhooksTookan.Handlers",
        Tags = new[] { "WebhooksTookanEndpoint" })
    ]
    public override async Task<ActionResult> HandleAsync([FromBody] TookanWebhook command,
        CancellationToken cancellationToken = default)
    {
        var courierTaskFound =
            await _courierTaskRepository.GetCourierTask(command.job_id.ToString(), cancellationToken);

        if (courierTaskFound is null)
        {
            throw new Exception("Courier task not found");
        }

        CheckTookanJobStatus(courierTaskFound, command.job_status);

        await _courierTaskRepository.UpdateCourierTask(courierTaskFound, cancellationToken);

        var newEvent = new CourierTaskUpdatedEventList()
        {
            OrganizationId = courierTaskFound.OrganizationId,
            Body = new CourierTaskUpdatedEvent()
            {
                CourierTaskId = courierTaskFound.Id,
                AssignedAt = courierTaskFound.AssignedAt,
                AcceptedAt = courierTaskFound.AcceptedAt,
                StartedAt = courierTaskFound.StartedAt,
                FailedAt = courierTaskFound.FailedAt,
                SucceededAt = courierTaskFound.SucceededAt,
                CanceledAt = courierTaskFound.CanceledAt
            }
        };

        await _eventBus.PublishAsync(
            newEvent
        );
        return Ok();
    }

    private static void CheckTookanJobStatus(CourierTask courierTask, TookanJobStatus jobStatus)
    {
        switch (jobStatus)
        {
            case TookanJobStatus.Assigned:
                courierTask.AssignedAt = DateTime.Now;
                break;

            case TookanJobStatus.AcceptedOrAcknowledged:
                courierTask.AcceptedAt = DateTime.Now;
                break;

            case TookanJobStatus.Started:
                courierTask.StartedAt = DateTime.Now;
                break;

            case TookanJobStatus.Failed:
                courierTask.FailedAt = DateTime.Now;
                break;

            case TookanJobStatus.Successful:
                courierTask.SucceededAt = DateTime.Now;
                break;

            case TookanJobStatus.Cancel:
                courierTask.CanceledAt = DateTime.Now;
                break;
        }

        courierTask.LastStateChangeAt = DateTime.Now;
    }
}