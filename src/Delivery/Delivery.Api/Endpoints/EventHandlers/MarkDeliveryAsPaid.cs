using Ardalis.ApiEndpoints;
using Dapr;
using Delivery.Domain.Entities;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Domain.Interfaces.Services;
using Delivery.Domain.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Events;
using Sitec.Delivery.EventBus.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Api.Endpoints.EventHandlers;

public class MarkDeliveryAsPaid : EndpointBaseAsync
    .WithRequest<BillPaidEvent>
    .WithActionResult
{
    private readonly IEventBus _eventBus;
    private readonly ICourierCommunicationService _courierService;
    private readonly ICourierTasksRepository _courierTasksRepository;

    public MarkDeliveryAsPaid(
        IEventBus eventBus,
        ICourierCommunicationService courierService,
        ICourierTasksRepository courierTasksRepository
    )
    {
        _courierService = courierService;
        _eventBus = eventBus;
        _courierTasksRepository = courierTasksRepository;
    }

    [Topic(GlobalConstanst.DAPR_PUBSUB_NAME, "BillPaidEvent")]
    [HttpPost("markaspaid/")]
    //[Authorize(Policy = "MachinePolicy")]
    [SwaggerOperation(
        Summary = "Create a pickup and delivery task",
        Description = "Create a pickup and delivery task",
        OperationId = "Tasks.CreateTask",
        Tags = new[] { "TasksEndpoint" })
    ]
    public override async Task<ActionResult> HandleAsync([FromBody] BillPaidEvent command,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("courier task payment received");
        var courierTaskFound =
            await _courierTasksRepository.GetForBillId(command.BillId, command.OrganizationId, cancellationToken);

        if (courierTaskFound is null)
        {
            return NotFound("Courier task not found");
        }

        courierTaskFound.MarkAsPaid();

        var externalPickUpAndDelivery = CreateExternalPickUpAndDelivery(courierTaskFound);

        var response = await _courierService.CreatePickupAndDeliveryAsync(externalPickUpAndDelivery);

        courierTaskFound.SetExternalTaskInformation(
            response.ExternalTaskId.ToString(),
            response.PickupTrackingLink,
            response.DeliveryTrackingLink
        );
        await _courierTasksRepository.UpdateAsync(courierTaskFound, cancellationToken);
        var newEvent = new CourierTaskUpdatedEventList()
        {
            OrganizationId = courierTaskFound.OrganizationId,
            Body = new CourierTaskUpdatedEvent()
            {
                CourierTaskId = courierTaskFound.Id,
                CompletedAt = courierTaskFound.AssignedAt,
            }
        };

        await _eventBus.PublishAsync(
            newEvent
        );
        return Ok();
    }

    private CreateExternalPickupAndDeliveryCommand CreateExternalPickUpAndDelivery(CourierTask courierTask)
    {
        var pickUpAndDelivery = new CreateExternalPickupAndDeliveryCommand();

        var deliveryAddress = new CreateExternalPickupAndDeliveryAddressCommandBody();

        var originAddress = new CreateExternalPickupAndDeliveryAddressCommandBody();

        deliveryAddress.ClientName = courierTask.DestinationClientName;
        deliveryAddress.Latitude = courierTask.DestinationLatitude;
        deliveryAddress.Longitude = courierTask.DestinationLongitude;
        deliveryAddress.PhoneNumber = courierTask.DestinationPhoneNumber;
        deliveryAddress.Reference = courierTask.DestinationAddressReference;

        originAddress.ClientName = courierTask.OriginClientName;
        originAddress.Latitude = courierTask.OriginLatitude;
        originAddress.Longitude = courierTask.OriginLongitude;
        originAddress.PhoneNumber = courierTask.OriginPhoneNumber;
        originAddress.Reference = courierTask.OriginAddressReference;

        pickUpAndDelivery.DeliveryAddress = deliveryAddress;

        pickUpAndDelivery.OriginAddress = originAddress;

        return pickUpAndDelivery;
    }
}