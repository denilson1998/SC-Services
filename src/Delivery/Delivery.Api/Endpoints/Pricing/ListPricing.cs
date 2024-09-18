using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using Delivery.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Contracts;
using SharedKernel.DataFilters;
using Swashbuckle.AspNetCore.Annotations;

namespace Delivery.Api.Endpoints.Pricing;

public class ListPricing : EndpointBaseAsync
    .WithRequest<ListPricingRequest>
    .WithActionResult<PagedResponse<ListPricingResult>>
{
    private readonly IMapper _mapper;
    private readonly IPricingRepository _pricingRepository;

    public ListPricing(IMapper mapper,
        IPricingRepository pricingRepository
    )
    {
        _mapper = mapper;
        _pricingRepository = pricingRepository;
    }

    [HttpGet("/pricing")]
    [Authorize(Policy = "UserWithoutParameters")]
    [SwaggerOperation(
        Summary = "List Pricing",
        Description = "List Pricing",
        OperationId = "Pricing.List",
        Tags = new[] { "PricingEndpoint" })
    ]
    public override async Task<ActionResult<PagedResponse<ListPricingResult>>> HandleAsync(
        [FromQuery] ListPricingRequest request, CancellationToken cancellationToken = default)
    {
        var query = _pricingRepository.GetAllPricingQuery();

        var pricing = await query.QueryPaged(request).ToListAsync(cancellationToken);

        var pricingCount = _pricingRepository.CountPricing(cancellationToken);

        var pricingResult = pricing.ConvertAll(b => _mapper.Map<ListPricingResult>(b));
        var result = new PagedResponse<ListPricingResult>(
            pricingResult,
            request.Skip,
            request.Limit,
            pricingCount
        );
        return Ok(result);
    }
}