using Ardalis.ApiEndpoints;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using SharedKernel.DataFilters;
using System.Threading;
using System.Threading.Tasks;
using SharedKernel.Contracts;
using Delivery.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Delivery.Api.Endpoints.CourierTasks;

public class ListCourierTasks : EndpointBaseAsync
    .WithRequest<ListCourierTasksRequest>
    .WithActionResult<PagedResponse<ListCourierTasksResult>>
{
    private readonly IMapper _mapper;
    private readonly ICourierTasksRepository _courierTasksRepository;

    public ListCourierTasks(IMapper mapper,
        ICourierTasksRepository courierTasksRepository
    )
    {
        _mapper = mapper;
        _courierTasksRepository = courierTasksRepository;
    }

    [HttpGet("/organizations/{OrganizationId}/courier-tasks")]
    [HttpGet("/courier-tasks")]
    [Authorize(Policy = "UserM2MPolicy")]
    [SwaggerOperation(
        Summary = "List CourierTasks",
        Description = "List CourierTasks",
        OperationId = "CourierTasks.List",
        Tags = new[] { "CourierTasksEndpoint" })
    ]
    public override async Task<ActionResult<PagedResponse<ListCourierTasksResult>>> HandleAsync(
        [FromQuery] ListCourierTasksRequest request, CancellationToken cancellationToken = default)
    {
        if (request.OrganizationId == null) return NotFound();
        var courierTasksQuery = _courierTasksRepository.GetAllCourierTaskQuery((int)request.OrganizationId);
        var courierTasks = await courierTasksQuery
            .QueryPaged(request)
            .ToListAsync(cancellationToken);

        var courierTasksCount = await _courierTasksRepository.CountCourierTask(cancellationToken);

        var courierTasksResult = courierTasks.ConvertAll(b => _mapper.Map<ListCourierTasksResult>(b));
        var result = new PagedResponse<ListCourierTasksResult>(
            courierTasksResult,
            request.Skip,
            request.Limit,
            courierTasksCount
        );
        return Ok(result);
    }
}