using Delivery.Domain.Entities;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Domain.Interfaces.Services;
using Delivery.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using SharedKernel.Contracts;
using SharedKernel.Events;
using Sitec.Delivery.EventBus.Abstractions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Delivery.Api.Workers;

[DisallowConcurrentExecution]
public class SyncPaidBills : IJob
{
    private readonly ILogger<SyncPaidBills> _logger;
    private readonly HttpClient _httpClient;
    private readonly IPaymentsCommunicationService _paymentsService;
    private readonly IWorkerRepository _workerRepository;
    private readonly ICourierTasksRepository _courierTasksRepository;
    private readonly IEventBus _eventBus;

    public SyncPaidBills(
        ILogger<SyncPaidBills> logger,
        HttpClient httpClient,
        IPaymentsCommunicationService paymentsService,
        IWorkerRepository workerRepository,
        ICourierTasksRepository courierTasksRepository,
        IEventBus eventBus
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _paymentsService = paymentsService;
        _workerRepository = workerRepository;
        _courierTasksRepository = courierTasksRepository;
        _eventBus = eventBus;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // crear tabla de workers y última corrida
        var workerFound = await _workerRepository.GetWorkerByName(nameof(SyncPaidBills));

        if (workerFound is null)
        {
            workerFound = new Worker()
            {
                Name = nameof(SyncPaidBills),
            };
            await _workerRepository.CreateWorkerAsync(workerFound);
        }

        var since = workerFound.DataSyncedSince?.AddMinutes(-5);
        var getBillsCommand = new ListBillsQueryFilters
        {
            IsCompleted = true,
            Since = since,

        };

        var courierTasks = await _courierTasksRepository.GetAllCourierTaskPaidAt();

        var billForEvent = courierTasks.Select(c => new BillPaidEvent(c.OrganizationId, c.BillId)).ToList();

        // TODO usar un Task.WhenAll ?
        // TODO revisar si es mejor single events o batch events como usamos para tookan
        billForEvent.ForEach(async b =>
            await _eventBus.PublishAsync(b)
        );

        workerFound.DataSyncedSince = DateTime.Now;

        await _workerRepository.UpdateWorkerAsync(workerFound);
    }
}