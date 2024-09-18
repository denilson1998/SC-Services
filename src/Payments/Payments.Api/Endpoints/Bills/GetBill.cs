using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Domain.Interfaces.Repositories;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Payments.Api.Endpoints.Bills;

public class GetBills : EndpointBaseAsync
    .WithRequest<GetBillCommand>
    .WithActionResult<GetBillResult>
{
    private readonly IMapper _mapper;
    private readonly IBillRepository _billRepository;

    public GetBills(IMapper mapper,
         IBillRepository billRepository
    )
    {
        _mapper = mapper;
        _billRepository = billRepository;
    }

    [HttpGet("/organizations/{OrganizationId}/bills/{BillId}")]
    [Authorize(Policy = "UserM2MPolicy")]
    [SwaggerOperation(
        Summary = "Get bill by Id",
        Description = "Get bill by Id",
        OperationId = "Bills.Get",
        Tags = new[] { "BillsEndpoint" })
    ]
    public override async Task<ActionResult<GetBillResult>> HandleAsync([FromRoute] GetBillCommand request, CancellationToken cancellationToken)
    {
        var billFound = await _billRepository.GetBillByIdIncludeQrPaymentsAndBankQrs(request.BillId, request.OrganizationId, cancellationToken);

        if (billFound is null)
        {
            return NotFound("The bill was not Found!");
        }

        var result = _mapper.Map<GetBillResult>(billFound);

        return Ok(result);
    }
}