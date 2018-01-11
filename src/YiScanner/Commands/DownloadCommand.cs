using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Archive;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Monitoring;

namespace Wikiled.YiScanner.Commands
{
    /// <summary>
    ///     Download -Cameras=1080i -Hosts=192.168.0.202 -Compress -Out=c:\out -Scan=10 [-Archive=2]
    /// </summary>
    [Description("Download video from camera")]
    public class DownloadCommand : BaseCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public DownloadCommand(FtpConfig ftpConfig)
            : base(ftpConfig)
        {
        }

        protected override IPredicate ConstructPredicate()
        {
            return new NullPredicate();
        }

        protected override void ProcessFtp(IDestinationFactory factory)
        {
            var downloaders = factory.GetDestinations();
            if (downloaders.Length == 0)
            {
                return;
            }

            var tasks = downloaders.Select(ftpDownloader => ftpDownloader.Download());
            var archiving = new DeleteArchiving();
            if (Archive.HasValue)
            {
                log.Info("Archiving...");
                archiving.Archive(Out, TimeSpan.FromDays(Archive.Value));
            }

            Task.WhenAll(tasks).Wait();
        }
    }
}
