using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Delivery.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Delivery.Api.Endpoints.Pricing;

public class Create : EndpointBaseAsync
    .WithRequest<CreatePricingCommand>
    .WithActionResult<CreatePricingResponse>
{
    private readonly IPricingRepository _pricingRepository;
    private readonly IMapper _mapper;

    public Create(
        IPricingRepository pricingRepository,
        IMapper mapper
    )
    {
        _pricingRepository = pricingRepository;
        _mapper = mapper;
    }

    [HttpPost("pricings")]
    [Authorize(Policy = "UserWithoutParameters")]
    [SwaggerOperation(
        Summary = "Create Pricing",
        Description = "Create Pricing",
        //OperationId = "Tasks.CreateTask",
        //Tags = new[] { "TasksEndpoint" })
        OperationId = "Pricings.CreatePricing",
        Tags = new[] { "PricingsEndpoint" })
    ]
    public override async Task<ActionResult<CreatePricingResponse>> HandleAsync([FromBody] CreatePricingCommand command,
        CancellationToken cancellationToken = default)
    {
        var newPricing = new Domain.Entities.Pricing(
            command.Alias,
            command.PricePerEstimatedKilometer,
            command.PricePerEstimatedMinute,
            command.MinimumPrice
        );

        var pricingResult = await _pricingRepository.CreateAsync(newPricing, cancellationToken);

        var result = _mapper.Map<CreatePricingResponse>(pricingResult);
        return Created("", result);
    }
}