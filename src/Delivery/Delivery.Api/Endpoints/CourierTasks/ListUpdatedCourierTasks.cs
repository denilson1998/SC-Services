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

namespace Delivery.Api.Endpoints.CourierTasks;

public class ListUpdatedCourierTasks : EndpointBaseAsync
    .WithRequest<ListUpdatedCourierTasksCommand>
    .WithActionResult<PagedResponse<ListUpdatedCourierTasksResponse>>
{
    private readonly IMapper _mapper;
    private readonly ICourierTasksRepository _courierTasksRepository;

    public ListUpdatedCourierTasks(IMapper mapper,
        ICourierTasksRepository courierTasksRepository
    )
    {
        _mapper = mapper;
        _courierTasksRepository = courierTasksRepository;
    }

    [HttpGet("/updated-courier-tasks")]
    //[Authorize(Policy = "UserM2MPolicy")]
    [SwaggerOperation(
        Summary = "List of updated CourierTasks",
        Description = "List of updated CourierTasks",
        OperationId = "CourierTasks.UpdatedList",
        Tags = new[] { "CourierTasksEndpoint" })
    ]
    public override async Task<ActionResult<PagedResponse<ListUpdatedCourierTasksResponse>>> HandleAsync([FromQuery]
        ListUpdatedCourierTasksCommand request, CancellationToken cancellationToken = default)
    {
        var courierTasksQuery =
            _courierTasksRepository.GetLastUpdatedCourierTasksQuery(request.LastUpdated);
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