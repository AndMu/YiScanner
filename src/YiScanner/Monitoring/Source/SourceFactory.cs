using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;
using Wikiled.YiScanner.Downloader;
using Wikiled.YiScanner.Monitoring.Config;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public class SourceFactory : ISourceFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IPredicate filePredicate;

        private readonly MonitoringConfig config;

        private readonly List<IDownloader> fixedDownloaders = new List<IDownloader>();

        private readonly ConcurrentDictionary<IPAddress, IDownloader> trackingInformation = new ConcurrentDictionary<IPAddress, IDownloader>();

        private readonly IDestination destination;

        public SourceFactory(MonitoringConfig config, IPredicate filePredicate)
        {
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => filePredicate, filePredicate);
            this.config = config;
            this.filePredicate = filePredicate;
            destination = ConstructDestination();
            if (config.Server != null)
            {
                fixedDownloaders.Add(new FileDownloader(config.Server, destination, filePredicate));
            }
        }

        public IEnumerable<IDownloader> GetSources(IHostManager manager)
        {
            log.Info("Downloading...");
            return fixedDownloaders.Union(manager.GetHosts().Select(item => ConstructDownloader(item, destination)));
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
            if (!trackingInformation.TryGetValue(host.Address, out var downloader))
            {
                var tracking = new HostTracking(config.YiFtp, host);
                downloader = new FtpDownloader(tracking, destination, filePredicate);
                trackingInformation[host.Address] = downloader;
            }

            return downloader;
        }
    }
}
