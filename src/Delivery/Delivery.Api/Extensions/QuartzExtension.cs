using System;
using Delivery.Api.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Delivery.Api.Extensions;

public static class ServiceCollectionQuartzConfiguratorExtensions
{
    public static IServiceCollection AddQuartzExtension(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();

            options.UseSimpleTypeLoader();
            options.UseDedicatedThreadPool(poolOptions => { poolOptions.MaxConcurrency = 10; });

            options.UsePersistentStore(storeOptions =>
            {
                storeOptions.UseProperties = true;
                storeOptions.RetryInterval = TimeSpan.FromSeconds(3);

                storeOptions.UseJsonSerializer();

                storeOptions.UseClustering(clusterOptions =>
                {
                    clusterOptions.CheckinMisfireThreshold = TimeSpan.FromSeconds(1);
                    clusterOptions.CheckinInterval = TimeSpan.FromSeconds(1);
                });

                storeOptions.UseSqlServer(providerOptions =>
                {
                    providerOptions.ConnectionString = configuration.GetConnectionString("DeliveryDb")!;
                });
            });

            options.ScheduleJob<DeleteUnusedJobs>(
                trigger => trigger
                    .StartNow()
                    .ForJob("DeleteUnusedJobs")
                    .WithIdentity($"{nameof(DeleteUnusedJobs)}_trigger")
                    .WithCronSchedule("0/7 * * * * ?", x => x.WithMisfireHandlingInstructionFireAndProceed()),
                job => job
                    .WithIdentity("DeleteUnusedJobs")
                    .PersistJobDataAfterExecution()
                    .DisallowConcurrentExecution()
            );

            options.ScheduleJob<SyncTookanCourierTasks>(
                trigger => trigger
                    .StartNow()
                    .ForJob($"{nameof(SyncTookanCourierTasks)}_job")
                    .WithIdentity($"{nameof(SyncTookanCourierTasks)}_trigger")
                    .WithCronSchedule("0/5 * * * * ?"),
                job => job
                    .WithIdentity($"{nameof(SyncTookanCourierTasks)}_job")
                    .PersistJobDataAfterExecution()
                    .DisallowConcurrentExecution()
            );


            options.ScheduleJob<SyncPaidBills>(
                trigger => trigger
                    .StartNow()
                    .ForJob($"{nameof(SyncPaidBills)}_job")
                    .WithIdentity($"{nameof(SyncPaidBills)}_trigger")
                    .WithCronSchedule("0/8 * * * * ?", x => x.WithMisfireHandlingInstructionFireAndProceed()),
                job => job
                    .WithIdentity($"{nameof(SyncPaidBills)}_job")
                    .PersistJobDataAfterExecution()
                    .DisallowConcurrentExecution()
            );
        });
        services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });
        return services;
    }
}