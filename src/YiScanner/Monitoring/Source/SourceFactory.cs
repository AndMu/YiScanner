using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public class SourceFactory : ISourceFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IPredicate filePredicate;

        private readonly FtpConfig ftpConfig;

        private readonly ConcurrentDictionary<IPAddress, HostTracking> trackingInformation = new ConcurrentDictionary<IPAddress, HostTracking>();

        public SourceFactory(FtpConfig ftpConfig, IScanConfig config, IPredicate filePredicate)
        {
            Guard.NotNull(() => ftpConfig, ftpConfig);
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => filePredicate, filePredicate);
            this.ftpConfig = ftpConfig;
            Config = config;
            this.filePredicate = filePredicate;
        }

        public IScanConfig Config { get; }

        public IEnumerable<IFtpDownloader> GetSources(IHostManager manager)
        {
            var destination = ConstructDestination();
            log.Info("Download from camera(s)");
            return manager.GetHosts().Select(item => ConstructDownloader(item, destination));
        }

        private IDestination ConstructDestination()
        {
            IDestination destination = Config.Images ? (IDestination)new PictureFileDestination(Config.Out) : new FileDestination(Config.Out);
            if (Config.Compress)
            {
                destination = ChainedPriorActionDestination.CreateCompressed(destination);
            }

            if (Config.Action != null)
            {
                destination = ChainedPostActionDestination.CreateAction(destination, Config.Action);
            }

            return destination;
        }

        private IFtpDownloader ConstructDownloader(Host host, IDestination destination)
        {
            if (!trackingInformation.TryGetValue(host.Address, out var tracking))
            {
                tracking = new HostTracking(ftpConfig, host);
                trackingInformation[host.Address] = tracking;
            }

            return new FtpDownloader(
                tracking,
                destination,
                filePredicate);
        }
    }
}
