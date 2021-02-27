using JMW.Google.OnHub.API.Data;
using JMW.Google.OnHub.API.Model;
using JMW.Google.OnHub.API.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace JMW.Google.OnHub.API.Schedule
{
    public static class CollectJobExtensions
    {
        public static IServiceCollectionQuartzConfigurator AddCollectJob(this IServiceCollectionQuartzConfigurator quartz, string cron)
        {
            quartz.AddJob<CollectJob>(opts => opts.WithIdentity(nameof(CollectJob)));

            quartz.AddTrigger(opts => opts
                .ForJob(nameof(CollectJob))
                .WithIdentity(nameof(CollectJob))
                .WithCronSchedule(cron,
                    b => b.WithMisfireHandlingInstructionDoNothing() // if it misfires, update to the next fire time and proceed as normal
                )
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(15))); // give the service time to start... and do validation

            return quartz;
        }
    }

    [DisallowConcurrentExecution]
    public class CollectJob : IJob
    {
        private readonly ILogger<CollectJob> logger;
        private readonly IOptionsMonitor<CollectionOptions> opts;
        private readonly ApplicationContext context;

        public CollectJob(
            ILogger<CollectJob> logger,
            IOptionsMonitor<CollectionOptions> opts,
            ApplicationContext context)
        {
            this.logger = logger;
            this.opts = opts;
            this.context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("Begginning OnHub Collection.");

            var target = IPAddress.Parse(this.opts.CurrentValue.Target);

            var data = await OnHubApi.GetData(target);
            var dt = DateTimeOffset.UtcNow;

            foreach (var arp in data.ArpCache.Cast<Arp>())
            {
                arp.SeenFrom = dt;
                arp.SeenTo = dt;
                await this.context.Upsert(arp)
                    // duplicate checking fields
                    .On(v => new { v.IpAddress, v.HwAddress })
                    .WhenMatched((v) => new Arp
                    {
                        SeenTo = v.SeenTo
                    })
                    .RunAsync();
            }

            this.context.SaveChanges();
        }
    }
}