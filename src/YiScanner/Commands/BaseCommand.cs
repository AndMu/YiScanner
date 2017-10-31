using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Commands
{
    public abstract class BaseCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly FtpConfiguration ftpConfiguration;

        protected BaseCommand(FtpConfiguration ftpConfiguration)
        {
            Guard.NotNull(() => ftpConfiguration, ftpConfiguration);
            this.ftpConfiguration = ftpConfiguration;
        }

        [Required]
        [Description("List of camera names")]
        public string Cameras { get; set; }

        [Required]
        [Description("List of camera hosts")]
        public string Hosts { get; set; }

        [Description("Compress video files")]
        public bool Compress { get; set; }

        [Required]
        [Description("File destination")]
        public string Out { get; set; }

        [Description("Archive video after days")]
        public int? Archive { get; set; }

        public override void Execute()
        {
            log.Info("Starting camera download...");
            if (string.IsNullOrEmpty(Cameras))
            {
                log.Error("Invalid camera(s) names");
                return;
            }

            if (string.IsNullOrEmpty(Hosts))
            {
                log.Error("Invalid camera(s) hosts");
                return;
            }

            var listOfCameras = Cameras.Split(',');
            var listOfHosts = Hosts.Split(',');
            if (listOfHosts.Length != listOfCameras.Length)
            {
                log.Error("List of camera names and hosts does not match");
                return;
            }

            log.Info("Download from {0} camera(s)", listOfHosts.Length);
            var ftpDownloaders = new List<FtpDownloader>(listOfHosts.Length);
            IDestination desitination = new FileDestination(Out);
            if (Compress)
            {
                desitination = new CompressedDestination(desitination);
            }

            for (int i = 0; i < listOfCameras.Length; i++)
            {
                ftpDownloaders.Add(
                    new FtpDownloader(
                        ftpConfiguration,
                        new CameraDescription(listOfCameras[i], listOfHosts[i]),
                        desitination,
                        ConstructPredicate()));
            }

            ProcessFtp(ftpDownloaders);
        }

        protected abstract IPredicate ConstructPredicate();

        protected abstract void ProcessFtp(List<FtpDownloader> downloaders);
    }
}
