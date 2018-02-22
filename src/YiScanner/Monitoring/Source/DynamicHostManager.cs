using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Monitoring.Config;
using Wikiled.YiScanner.Network;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public class DynamicHostManager : IHostManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<IPAddress, Host> result = new ConcurrentDictionary<IPAddress, Host>();

        private readonly IDisposable subscription;

        private readonly INetworkScanner scanner;

        private readonly MonitoringConfig config;

        public DynamicHostManager(MonitoringConfig config, INetworkScanner scanner, IScheduler scheduler)
        {
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => scanner, scanner);
            Guard.NotNull(() => scheduler, scheduler);

            this.scanner = scanner;
            this.config = config;
            subscription = Observable.FromAsync(ScanFtp, scheduler).Delay(TimeSpan.FromSeconds(10), scheduler).Repeat().SubscribeOn(scheduler).Subscribe();
        }

        public void Dispose()
        {
            subscription.Dispose();
        }

        public IEnumerable<Host> GetHosts()
        {
            return result.Values;
        }

        private async Task<bool> ScanFtp()
        {
            log.Debug("ScanFtp");
            ConcurrentDictionary<IPAddress, Host> thisCycle = new ConcurrentDictionary<IPAddress, Host>();
            await scanner.FindAddresses(config.AutoDiscovery.NetworkMask, 21)
                         .ForEachAsync(
                             item =>
                                 {
                                     thisCycle[item.Address] = item;
                                     if (!result.ContainsKey(item.Address))
                                     {
                                         log.Info("Adding new host: {0}", item.Address);
                                         result[item.Address] = item;
                                     }
                                 })
                         .ConfigureAwait(false);
            foreach (var host in result.Keys.ToArray())
            {
                if (!thisCycle.ContainsKey(host))
                {
                    log.Info("Removing: {0}", host);
                    result.TryRemove(host, out _);
                }
            }

            return true;
        }
    }
}
