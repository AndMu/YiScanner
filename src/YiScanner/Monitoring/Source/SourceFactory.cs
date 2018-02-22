using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;
using Wikiled.YiScanner.Monitoring.Config;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public class SourceFactory : ISourceFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IPredicate filePredicate;

        private readonly MonitoringConfig config;

        private readonly ConcurrentDictionary<IPAddress, HostTracking> trackingInformation = new ConcurrentDictionary<IPAddress, HostTracking>();

        public SourceFactory(MonitoringConfig config, IPredicate filePredicate)
        {
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => filePredicate, filePredicate);
            this.config = config;
            this.filePredicate = filePredicate;
        }

        public IEnumerable<IDownloader> GetSources(IHostManager manager)
        {
            var destination = ConstructDestination();
            log.Info("Download from camera(s)");
            return manager.GetHosts().Select(item => ConstructDownloader(item, destination));
        }

        private IDestination ConstructDestination()
        {
            IDestination destination = config.Output.Images ? (IDestination)new PictureFileDestination(config.Output.Out) : new FileDestination(config.Output.Out);
            if (config.Output.Compress)
            {
                destination = ChainedPriorActionDestination.CreateCompressed(destination);
            }

            if (config.Action != null)
            {
                destination = ChainedPostActionDestination.CreateAction(destination, config.Action);
            }

            return destination;
        }

        private IDownloader ConstructDownloader(Host host, IDestination destination)
        {
            if (!trackingInformation.TryGetValue(host.Address, out var tracking))
            {
                tracking = new HostTracking(config.YiFtp, host);
                trackingInformation[host.Address] = tracking;
            }

            return new FtpDownloader(
                tracking,
                destination,
                filePredicate);
        }
    }
}
