using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Delivery.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Delivery.Api.Endpoints.Pricing;

public class ActivatePricing : EndpointBaseAsync
    .WithRequest<int>
    .WithActionResult<ListPricingResult>
{
    private readonly IMapper _mapper;
    private readonly IPricingRepository _pricingRepository;

    public ActivatePricing(
        IMapper mapper,
        IPricingRepository pricingRepository
    )
    {
        _mapper = mapper;
        _pricingRepository = pricingRepository;
    }

    [HttpPost("/pricing/{pricingId}/activate")]
    [Authorize(Policy = "UserWithoutParameters")]
    [SwaggerOperation(
        Summary = "Activates a pricing",
        Description = "Activates a pricing",
        OperationId = "Pricing.Activate",
        Tags = new[] { "PricingEndpoint" })
    ]
    public override async Task<ActionResult<ListPricingResult>> HandleAsync([FromRoute] int pricingId,
        CancellationToken cancellationToken = default)
    {
        var activePricing = await _pricingRepository.GetForStateActivePricing(true, cancellationToken);

        activePricing.ForEach(p => p.IsActive = false);

        var pricingFound = await _pricingRepository.GetForId(pricingId, cancellationToken);

        if (pricingFound is null)
        {
            return NotFound("Pricing not found");
        }

        pricingFound.IsActive = true;

        await _pricingRepository.UpdateAsync(pricingFound, cancellationToken);

        return _mapper.Map<ListPricingResult>(pricingFound);
    }
}