using JMW.Google.OnHub.API.Data;
using JMW.Google.OnHub.API.Model;
using JMW.Google.OnHub.API.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IOptionsMonitor<CollectionOptions> collectOptions;
        private readonly IOptionsMonitor<CacheOptions> cacheOptions;
        private readonly ApplicationContext context;

        public CollectJob(
            ILogger<CollectJob> logger,
            IOptionsMonitor<CollectionOptions> collectOptions,
            IOptionsMonitor<CacheOptions> cacheOptions,
            ApplicationContext context)
        {
            this.logger = logger;
            this.collectOptions = collectOptions;
            this.cacheOptions = cacheOptions;
            this.context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("Beginning OnHub Collection.");
            var sw = new Stopwatch();
            sw.Start();
            var step = new Stopwatch();
            var dt = DateTimeOffset.UtcNow;

            // collect the data
            step.Start();
            var target = IPAddress.Parse(this.collectOptions.CurrentValue.Target);
            var data = await OnHubApi.GetData(target);
            logger.LogInformation($"(elapsedTime={step.Elapsed}, collectionTarget={target}) data retrieved");

            // store the data.
            step.Restart();
            await handleArp(data.ArpCache, dt);
            this.context.SaveChanges();
            logger.LogInformation($"(elapsedTime={step.Elapsed}, type={nameof(Arp)}) data stored");

            // save all the changes.
            logger.LogInformation($"(elapsedTime={sw.Elapsed}) OnHub Collection complete");
        }

        private async Task handleArp(IEnumerable<OnHub.Model.Arp> arpEntries, DateTimeOffset dt)
        {
            foreach (var arp in arpEntries)
            {
                // get entry
                var currentArp = await this.context.Arp.FindAsync(arp.IpAddress);
                if (currentArp == null) // its never been seen before...
                {
                    // store it and a history entry then exit, you'll all done.
                    var arpEntry = Arp.Convert(arp, dt, dt);
                    this.context.Arp.Add(arpEntry); // add to current cache.
                    this.context.ArpHistory.Add(ArpHistory.Convert(arpEntry)); // add a history record for the new entry.
                    continue;
                }

                // should it be deleted?
                var timeoutDate =
                    DateTimeOffset.UtcNow.Subtract(
                        new TimeSpan(cacheOptions.CurrentValue.CurrentTimeoutInDays, 0, 0, 0));
                if (currentArp.SeenTo < timeoutDate)
                { // yup, its too old, take it out.
                    this.context.Arp.Remove(currentArp);
                    continue;
                }

                // we're keeping it! update all the data
                if (currentArp.HwAddress != arp.HwAddress)
                {
                    // we use the current time here because if the hw address has changed, then the IP
                    //  has been reassigned to a new device, and the seen range needs to be reset.
                    currentArp.SeenFrom = dt; // history depends on this.
                }
                
                // now update the other info so the cache is fresh.
                currentArp.HwAddress = arp.HwAddress;
                currentArp.HwType = arp.HwType;
                currentArp.Mask = arp.Mask;
                currentArp.Flags = arp.Flags;
                currentArp.Interface = arp.Interface;
                currentArp.SeenTo = dt;

                this.context.Arp.Update(currentArp);

                // handle history.
                var historicalArp = await this.context.ArpHistory.FindAsync(currentArp.IpAddress, currentArp.HwAddress, currentArp.SeenFrom);
                if (historicalArp == null) // its never been seen before...
                {
                    // this means that the hw address and seenfrom are new, since we will have added a history record above
                    //   if the arp entry had never been seen before.  so add a new record.
                    this.context.ArpHistory.Add(ArpHistory.Convert(currentArp));
                    continue;
                }

                // should it be deleted?
                var historyTimeoutDate =
                    DateTimeOffset.UtcNow.Subtract(
                        new TimeSpan(cacheOptions.CurrentValue.HistoryTimeoutInDays, 0, 0, 0));
                if (historicalArp.SeenTo < historyTimeoutDate)
                {
                    this.context.ArpHistory.Remove(historicalArp);
                    continue;
                }

                // we're keeping it
                // the arp entry has been seen before, this is just an update of the seen range
                historicalArp.SeenTo = dt;
                this.context.ArpHistory.Update(historicalArp);
            }
        }
    }
}