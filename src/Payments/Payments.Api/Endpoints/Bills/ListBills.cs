using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Interfaces.Repositories;
using SharedKernel.Contracts;
using SharedKernel.Contracts.Payments;
using SharedKernel.DataFilters;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Api.Endpoints.Bills;

public class ListBills : EndpointBaseAsync
    .WithRequest<ListBillsCommand>
    .WithActionResult<PagedResponse<ListBillsResult>>
{
    private readonly IMapper _mapper;
    private readonly IBillRepository _billRepository;

    public ListBills(IMapper mapper,
        IBillRepository billRepository
    )
    {
        _mapper = mapper;
        _billRepository = billRepository;
    }

    [HttpGet("/organizations/{OrganizationId}/bills")]
    [HttpGet("/bills")]
    //[Authorize(Policy = "UserM2MPolicy")]
    [SwaggerOperation(
        Summary = "List bills",
        Description = "List bills",
        OperationId = "Bills.List",
        Tags = new[] { "BillsEndpoint" })
    ]
    public override async Task<ActionResult<PagedResponse<ListBillsResult>>> HandleAsync([FromQuery] ListBillsCommand request, CancellationToken cancellationToken)
    {
        var listBillsQuery = _billRepository.GetQueryable(request.OrganizationId, cancellationToken);

        if (request.IsCompleted is not null)
        {
            listBillsQuery = listBillsQuery.Where(b => b.IsCompleted == request.IsCompleted);
        }

        var billsFound = await listBillsQuery
            .QueryPaged(request)
            .ToListAsync(cancellationToken);

        var billsCount = await _billRepository.GetBillCount(request.OrganizationId, cancellationToken);

        var billsResult = billsFound.ConvertAll(b => _mapper.Map<ListBillsResult>(b));
        var result = new PagedResponse<ListBillsResult>(
            billsResult,
            request.Skip,
            request.Limit,
            billsCount
        );
        return Ok(result);
    }
}