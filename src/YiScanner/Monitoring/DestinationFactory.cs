using System.Collections.Generic;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Monitoring
{
    public class DestinationFactory : IDestinationFactory
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IScanConfig config;

        private readonly FtpConfig ftpConfig;

        private readonly IPredicate filePredicate;

        public DestinationFactory(FtpConfig ftpConfig, IScanConfig config, IPredicate filePredicate)
        {
            Guard.NotNull(() => ftpConfig, ftpConfig);
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => filePredicate, filePredicate);
            this.ftpConfig = ftpConfig;
            this.config = config;
            this.filePredicate = filePredicate;
        }

        public IFtpDownloader[] GetDestinations()
        {
            if (string.IsNullOrEmpty(config.Cameras))
            {
                log.Error("Invalid camera(s) names");
                return null;
            }

            if (string.IsNullOrEmpty(config.Hosts))
            {
                log.Error("Invalid camera(s) hosts");
                return null;
            }

            var listOfCameras = config.Cameras.Split(',');
            var listOfHosts = config.Hosts.Split(',');
            if (listOfHosts.Length != listOfCameras.Length)
            {
                log.Error("List of camera names and hosts does not match");
                return null;
            }

            log.Info("Download from {0} camera(s)", listOfHosts.Length);

            var ftpDownloaders = new List<FtpDownloader>(listOfHosts.Length);
            IDestination destination = config.Images ? (IDestination)new PictureFileDestination(config.Out) : new FileDestination(config.Out);
            if (config.Compress)
            {
                destination = ChainedPriorActionDestination.CreateCompressed(destination);
            }

            if (config.Action != null)
            {
                destination = ChainedPostActionDestination.CreateAction(destination, config.Action);
            }

            for (int i = 0; i < listOfCameras.Length; i++)
            {
                ftpDownloaders.Add(
                    new FtpDownloader(
                        ftpConfig,
                        new CameraDescription(listOfCameras[i], listOfHosts[i]),
                        destination,
                        filePredicate));
            }

            return ftpDownloaders.ToArray();
        }
    }
}
