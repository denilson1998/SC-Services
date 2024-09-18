using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Api.Workers;
using Quartz;

namespace Payments.Api.Extensions;
public static class ServiceCollectionQuartzConfiguratorExtensions
{
    public static IServiceCollection AddQuartzExtension(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(options =>
        {
            options.UseMicrosoftDependencyInjectionJobFactory();

            options.UseSimpleTypeLoader();
            options.UseDedicatedThreadPool(poolOptions =>
            {
                poolOptions.MaxConcurrency = 10;
            });

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
                    providerOptions.ConnectionString = configuration.GetConnectionString("PaymentsDb");
                });
            });

            options.ScheduleJob<DeleteUnusedJobs>(
                trigger => trigger
                    .StartNow()
                    .ForJob($"{nameof(DeleteUnusedJobs)}_job")
                    .WithIdentity($"{nameof(DeleteUnusedJobs)}_trigger")
                    .WithCronSchedule("0 * * * * ?", x => x.WithMisfireHandlingInstructionFireAndProceed()),
                job => job
                    .WithIdentity($"{nameof(DeleteUnusedJobs)}_job")
                    .PersistJobDataAfterExecution()
                    .DisallowConcurrentExecution()
            );

            options.ScheduleJob<SyncBankQrPayments>(
                trigger => trigger
                    .StartNow()
                    .ForJob($"{nameof(SyncBankQrPayments)}_job")
                    .WithIdentity($"{nameof(SyncBankQrPayments)}_trigger")
                    .WithCronSchedule("0/5 * * * * ?", x => x.WithMisfireHandlingInstructionFireAndProceed()),
                job => job
                    .WithIdentity($"{nameof(SyncBankQrPayments)}_job")
                    .PersistJobDataAfterExecution()
                    .DisallowConcurrentExecution()
            );
        });
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        return services;
    }
}