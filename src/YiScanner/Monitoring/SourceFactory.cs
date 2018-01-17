using System.Collections.Generic;
using NLog;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;

namespace Wikiled.YiScanner.Monitoring
{
    public class SourceFactory : SourceFactoryBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public SourceFactory(FtpConfig ftpConfig, IScanConfig config, IPredicate filePredicate)
            : base(ftpConfig, config, filePredicate)
        {
        }

        public override IEnumerable<IFtpDownloader> GetSources()
        {
            if (string.IsNullOrEmpty(Config.Cameras))
            {
                log.Error("Invalid camera(s) names");
                yield break;
            }

            if (string.IsNullOrEmpty(Config.Hosts))
            {
                log.Error("Invalid camera(s) hosts");
                yield break;
            }

            var listOfCameras = Config.Cameras.Split(',');
            var listOfHosts = Config.Hosts.Split(',');
            if (listOfHosts.Length != listOfCameras.Length)
            {
                log.Error("List of camera names and hosts does not match");
                yield break;
            }

            log.Info("Download from {0} camera(s)", listOfHosts.Length);
            var destination = ConstructDestination();
            for (int i = 0; i < listOfCameras.Length; i++)
            {
                yield return ConstructDownloader(listOfCameras[i], listOfHosts[i], destination);
            }
        }
    }
}
