using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Api.Workers;

[DisallowConcurrentExecution]
public class DeleteUnusedJobs : IJob
{
    private readonly ILogger<DeleteUnusedJobs> _logger;
    public DeleteUnusedJobs(ILogger<DeleteUnusedJobs> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("deleting jobs");
        _logger.LogInformation("deleting jobs");
        var currentJobs = new List<JobKey>()
        {
            new JobKey($"{nameof(DeleteUnusedJobs)}_trigger"),
            new JobKey($"{nameof(SyncBankQrPayments)}_trigger")
        };
        var databaseJobGroupNames = (await context.Scheduler.GetJobGroupNames()).ToList();

        var unusedJobs = new List<JobKey>();
        databaseJobGroupNames.ForEach(async d =>
        {
            var groupMatcher = GroupMatcher<JobKey>.GroupEquals(d);
            var databaseJobs = await context.Scheduler.GetJobKeys(groupMatcher);
            databaseJobs.ToList().ForEach(j =>
            {
                if (currentJobs.Contains(j))
                {
                    return;
                }
                unusedJobs.Add(j);
            });
        });
        await context.Scheduler.DeleteJobs(unusedJobs);
    }
}