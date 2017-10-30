using System;
using System.Collections.Generic;
using System.Net;
using NLog;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Commands
{
    /// <summary>
    /// Download -Cameras=1080i -Hosts=192.168.0.202 -Compress -Out=c:\out -Scan=10
    /// </summary>
    [Description("Download video from cameras")]
    public class DownloadCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private List<FtpDownloader> ftpDownloaders;

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

        [Required]
        [Description("Scan interval in second")]
        public int Scan { get; set; }

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
            ftpDownloaders = new List<FtpDownloader>(listOfHosts.Length);
            for (int i = 0; i < listOfCameras.Length; i++)
            {
                IDestination desitination = new FileDestination(Out);
                if (Compress)
                {
                    desitination = new CompressedDestination(desitination);
                }

                ftpDownloaders.Add(
                    new FtpDownloader(
                        new CameraDescription(listOfCameras[i], IPAddress.Parse(listOfHosts[i])),
                        desitination,
                        new Analyzer()));
            }

            log.Info("Press enter to stop processing");
            var observable = Observable.Interval(TimeSpan.FromSeconds(Scan))
                                       .Select(item => Download())
                                       .Replay();
            observable.Connect();
            Console.ReadLine();
        }

        private async Task Download()
        {
            log.Info("Starting download....");
            var tasks = ftpDownloaders.Select(ftpDownloader => ftpDownloader.Download());
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
