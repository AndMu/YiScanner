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
using Wikiled.YiScanner.Network;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public class DynamicHostManager : IHostManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<IPAddress, HostInformation> result = new ConcurrentDictionary<IPAddress, HostInformation>();

        private readonly IDisposable subscription;

        private readonly INetworkScanner scanner;

        private readonly IScanConfig config;

        public DynamicHostManager(IScanConfig config, INetworkScanner scanner, IScheduler scheduler)
        {
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => scanner, scanner);
            Guard.NotNull(() => scheduler, scheduler);

            this.scanner = scanner;
            this.config = config;
            subscription = Observable.FromAsync(ScanFtp, scheduler)
                                     .Delay(TimeSpan.FromSeconds(10), scheduler)
                                     .Repeat()
                                     .SubscribeOn(scheduler)
                                     .Subscribe();
        }

        public void Dispose()
        {
            subscription.Dispose();
        }

        public IEnumerable<HostInformation> GetHosts()
        {
            return result.Values;
        }

        private async Task<bool> ScanFtp()
        {
            log.Debug("ScanFtp");
            ConcurrentDictionary<IPAddress, HostInformation> thisCycle = new ConcurrentDictionary<IPAddress, HostInformation>();
            await scanner.FindAddresses(config.NetworkMask, 21)
                         .ForEachAsync(
                             item =>
                                 {
                                     thisCycle[item.Address] = item;
                                     result[item.Address] = item;
                                 })
                         .ConfigureAwait(false);
            foreach (var host in result.Keys.ToArray())
            {
                if (!thisCycle.ContainsKey(host))
                {
                    log.Debug("Removing: {0}", host);
                    result.TryRemove(host, out _);
                }
            }

            return true;
        }
    }
}
