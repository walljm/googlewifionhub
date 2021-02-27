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
            await handleArps(data.ArpCache, dt);
            this.context.SaveChanges();
            logger.LogInformation($"(elapsedTime={step.Elapsed}, type={nameof(Arp)}) data stored");

            step.Restart();
            await handleInterfaces(data.Interfaces, dt);
            this.context.SaveChanges();
            logger.LogInformation($"(elapsedTime={step.Elapsed}, type={nameof(Interface)}) data stored");

            
            step.Restart();
            await handleMacs(data.MacTable, dt);
            this.context.SaveChanges();
            logger.LogInformation($"(elapsedTime={step.Elapsed}, type={nameof(Mac)}) data stored");

            // save all the changes.
            logger.LogInformation($"(elapsedTime={sw.Elapsed}) OnHub Collection complete");
        }

        private async Task handleMacs(IEnumerable<OnHub.Model.Mac> entries, DateTimeOffset dt)
        {
            foreach (var mac in entries)
            {
                // get entry
                var current = await this.context.Mac.FindAsync(mac.HwAddress);
                if (current == null) // its never been seen before...
                {
                    // store it and a history entry then exit, you'll all done.
                    this.context.Mac.Add(Mac.ToMac(mac, dt, dt)); // add to current cache.
                    continue;
                }

                // should it be deleted?
                var timeoutDate =
                    DateTimeOffset.UtcNow.Subtract(
                        new TimeSpan(cacheOptions.CurrentValue.CurrentTimeoutInDays, 0, 0, 0));
                if (current.SeenTo < timeoutDate)
                { // yup, its too old, take it out.
                    this.context.Mac.Remove(current);
                    continue;
                }

                // now update the other info so the cache is fresh.
                this.context.Mac.Remove(current); // remove so we can replace with new one
                this.context.Mac.Add(Mac.ToMac(mac, current.SeenFrom, dt));
            }
        }

        private async Task handleArps(IEnumerable<OnHub.Model.Arp> entries, DateTimeOffset dt)
        {
            foreach (var arp in entries)
            {
                // get entry
                var currentArp = await this.context.Arp.FindAsync(arp.IpAddress);
                if (currentArp == null) // its never been seen before...
                {
                    // store it and a history entry then exit, you'll all done.
                    var arpEntry = Arp.ToArp(arp, dt, dt);
                    this.context.Arp.Add(arpEntry); // add to current cache.
                    this.context.ArpHistory.Add(ArpHistory.ToArpHistory(arpEntry)); // add a history record for the new entry.
                    continue;
                }

                // should it be deleted?
                var timeoutDate =
                    DateTimeOffset.UtcNow.Subtract(
                        new TimeSpan(cacheOptions.CurrentValue.CurrentTimeoutInDays, 0, 0, 0));
                if (currentArp.LastSeen < timeoutDate)
                { // yup, its too old, take it out.
                    this.context.Arp.Remove(currentArp);
                    continue;
                }

                // we're keeping it! update all the data
                if (currentArp.HwAddress != arp.HwAddress)
                {
                    // we use the current time here because if the hw address has changed, then the IP
                    //  has been reassigned to a new device, and the seen range needs to be reset.
                    currentArp.FirstSeen = dt; // history depends on this.
                }

                // now update the other info so the cache is fresh.
                currentArp.HwAddress = arp.HwAddress;
                currentArp.HwType = arp.HwType;
                currentArp.Mask = arp.Mask;
                currentArp.Flags = arp.Flags;
                currentArp.Interface = arp.Interface;
                currentArp.LastSeen = dt;

                this.context.Arp.Update(currentArp);

                // handle history.
                var historicalArp = await this.context.ArpHistory.FindAsync(currentArp.IpAddress, currentArp.HwAddress, currentArp.FirstSeen);
                if (historicalArp == null) // its never been seen before...
                {
                    // this means that the hw address and seenfrom are new, since we will have added a history record above
                    //   if the arp entry had never been seen before.  so add a new record.
                    this.context.ArpHistory.Add(ArpHistory.ToArpHistory(currentArp));
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

        private async Task handleInterfaces(IEnumerable<OnHub.Model.Interface> entries, DateTimeOffset dt)
        {
            foreach (var ifc in entries)
            {
                // get entry
                var current = await this.context.Interface.FindAsync(ifc.IfIndex);
                if (current == null) // its never been seen before...
                {
                    this.context.Interface.Add(Interface.ToInterface(ifc, dt, dt)); // add to current cache.
                    await this.handleInets(ifc, dt);
                    continue;
                }

                // should it be deleted?
                var timeoutDate =
                    DateTimeOffset.UtcNow.Subtract(
                        new TimeSpan(cacheOptions.CurrentValue.CurrentTimeoutInDays, 0, 0, 0));
                if (current.SeenTo < timeoutDate)
                { // yup, its too old, take it out.
                    this.context.Interface.Remove(current);
                    continue;
                }

                // now update the other info so the cache is fresh.
                this.context.Interface.Remove(current); // remove so we can replace with new one
                this.context.Interface.Add(Interface.ToInterface(ifc, current.SeenFrom, dt));
                await this.handleInets(ifc, dt);
            }
        }

        private async Task handleInets(OnHub.Model.Interface ifc, DateTimeOffset dt)
        {
            foreach (var info in ifc.Inet)
            {
                // get entry
                var current = await this.context.InterfaceInets.FindAsync(info.Inet, ifc.IfIndex);
                if (current == null) // its never been seen before...
                {
                    // store it and a history entry then exit, you'll all done.
                    this.context.InterfaceInets.Add(IpInfo.ToInetInfo(info, dt, dt)); // add to current cache.
                    continue;
                }

                var timeoutDate =
                    DateTimeOffset.UtcNow.Subtract(
                        new TimeSpan(cacheOptions.CurrentValue.CurrentTimeoutInDays, 0, 0, 0));
                if (current.SeenTo < timeoutDate)
                { // yup, its too old, take it out.
                    this.context.InterfaceInets.Remove(current);
                    continue;
                }

                // now update the other info so the cache is fresh.
                this.context.InterfaceInets.Remove(current); // remove so we can replace with new one
                this.context.InterfaceInets.Add(IpInfo.ToInetInfo(info, current.SeenFrom, dt));
            }
        }
    }
}