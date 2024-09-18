using Ardalis.ApiEndpoints;
using AutoMapper;
using Delivery.Domain.Entities;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constants;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Delivery.Api.Endpoints.CourierTasks;

public class Create : EndpointBaseAsync
    .WithRequest<CreateCourierTaskCommand>
    .WithActionResult<CreateCourierTaskResponse>
{
    private readonly IMapper _mapper;
    private readonly IPaymentsCommunicationService _paymentsService;
    private readonly IFareRepository _fareRepository;
    private readonly ICourierTasksRepository _courierTasksRepository;

    public Create(
        IMapper mapper,
        IPaymentsCommunicationService paymentsService,
        IFareRepository fareRepository,
        ICourierTasksRepository courierTasksRepository
    )
    {
        _paymentsService = paymentsService;
        _mapper = mapper;
        _fareRepository = fareRepository;
        _paymentsService = paymentsService;
        _courierTasksRepository = courierTasksRepository;
    }

    [HttpPost("organizations/{OrganizationId}/tasks/")]
    [Authorize(Policy = "UserM2MPolicy")]
    [SwaggerOperation(
        Summary = "Create a Courier Task",
        Description = "Create a Courier Task",
        OperationId = "CourierTasks.Create",
        Tags = new[] { "CourierTasksEndpoint" })
    ]
    public override async Task<ActionResult<CreateCourierTaskResponse>> HandleAsync(
        [FromRoute] CreateCourierTaskCommand command, CancellationToken cancellationToken = default)
    {
        var fareFound = await _fareRepository.GetForId(command.CreateCourierTaskCommandBody.FareId, cancellationToken);

        if (fareFound is null)
        {
            return BadRequest($"Fare with Id {command.CreateCourierTaskCommandBody.FareId} not found");
        }

        var courierTaskFound =
            await _courierTasksRepository.GetForFareId(command.CreateCourierTaskCommandBody.FareId, cancellationToken);

        if (courierTaskFound is not null)
        {
            return BadRequest($"Fare with Id {command.CreateCourierTaskCommandBody.FareId} is already used");
        }

        var createBillDto = new CreateBillDto()
        {
            Amount = fareFound.Price,
            CreateBankQr = true,
            Currency = Currency.Bolivianos,
            OrganizationId = command.OrganizationId
        };
        var newBill = await _paymentsService.CreateBillAsync(createBillDto);
        if (newBill is null) {
            return null;
        }

        var courierTask = new CourierTask(
            fareFound,
            newBill.Id,
            newBill.OrganizationId
        );

        courierTask.SetOriginAddress(
            command.CreateCourierTaskCommandBody.Origin.ClientName,
            command.CreateCourierTaskCommandBody.Origin.PhoneNumber,
            command.CreateCourierTaskCommandBody.Origin.AddressReference
        );

        courierTask.SetDestinationAddress(
            command.CreateCourierTaskCommandBody.Destination.ClientName,
            command.CreateCourierTaskCommandBody.Destination.PhoneNumber,
            command.CreateCourierTaskCommandBody.Destination.AddressReference
        );

        await _courierTasksRepository.CreateAsync(courierTask, cancellationToken);
        var result = _mapper.Map<CreateCourierTaskResponse>(courierTask);
        return Created("", result);
    }
}