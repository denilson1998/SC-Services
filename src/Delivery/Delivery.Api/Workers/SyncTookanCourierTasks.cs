using Delivery.Domain.Entities;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Domain.Interfaces.Services;
using Delivery.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SharedKernel.Events;
using Sitec.Delivery.EventBus.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.Api.Workers;

[DisallowConcurrentExecution]
public class SyncTookanCourierTasks : IJob
{
    private readonly ICourierCommunicationService _courierService;
    private readonly IEventBus _eventBus;
    private readonly IWebhookRepository _webhookRepository;
    private readonly ICourierTasksRepository _courierTasksRepository;

    public SyncTookanCourierTasks(
        ICourierCommunicationService courierService,
        IEventBus eventBus,
        IWebhookRepository webhookRepository,
        ICourierTasksRepository courierTasksRepository
    )
    {
        _courierService = courierService;
        _eventBus = eventBus;
        _webhookRepository = webhookRepository;
        _courierTasksRepository = courierTasksRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {

        var webhooksFound = _webhookRepository.GetAllWebhooks();

        var courierTasksFound = await _courierTasksRepository.GetAllCourierTaskByStatus();

        if (courierTasksFound.Count == 0)
        {
            return;
        }

        var jobIds = courierTasksFound.Select(c => c.ExternalTaskId).ToList();
        // TODO agregar loop (solo se puede de a 100)
        if (jobIds.Count == 0)
        {
            return;
        }

        var tookanTasks = await _courierService.GetCourierTasksAsync(jobIds);

        courierTasksFound.ForEach(c =>
        {
            var externalTaskFound = tookanTasks.Find(t => t.ExternalTaskId == c.ExternalTaskId);
            if (externalTaskFound is null)
            {
                return;
            }

            c.AssignedAt ??= externalTaskFound.AssignedAt;
            c.AcceptedAt ??= externalTaskFound.AcceptedAt;
            c.StartedAt ??= externalTaskFound.StartedAt;
            c.ArrivedAt ??= externalTaskFound.ArrivedAt;
            c.CompletedAt ??= externalTaskFound.CompletedAt;
            c.SucceededAt ??= externalTaskFound.SucceededAt;
            c.FailedAt ??= externalTaskFound.FailedAt;
            c.CanceledAt ??= externalTaskFound.CanceledAt;
        });

        var modifiedCourierTasks = _courierTasksRepository.GetAllCourierTaskModified();

        await _courierTasksRepository.UpdateRangeAsync();

        if (modifiedCourierTasks.Count > 0)
        {
            webhooksFound.Result.ForEach(w =>
            {
                var filteredCourierTasks = modifiedCourierTasks
                    .Where(c => c.OrganizationId == w.OrganizationId)
                    .ToList();
                filteredCourierTasks.ForEach(async f =>
                {
                    var data = new CourierTaskUpdatedEvent()
                    {
                        CourierTaskId = f.Id,
                        AssignedAt = f.AssignedAt,
                        AcceptedAt = f.AcceptedAt,
                        StartedAt = f.StartedAt,
                        FailedAt = f.FailedAt,
                        SucceededAt = f.SucceededAt,
                        CanceledAt = f.CanceledAt,
                    };
                    var newEvent = new CourierTaskUpdatedEventList()
                    {
                        OrganizationId = w.OrganizationId,
                        Body = data
                    };
                    await _eventBus.PublishAsync(
                        newEvent
                    );
                });
            });
        }
    }
}