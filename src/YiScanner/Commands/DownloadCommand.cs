using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Archive;
using Wikiled.YiScanner.Client.Predicates;

namespace Wikiled.YiScanner.Commands
{
    /// <summary>
    /// Download -Cameras=1080i -Hosts=192.168.0.202 -Compress -Out=c:\out -Scan=10
    /// </summary>
    [Description("Download video from camera")]
    public class DownloadCommand : BaseCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        protected override void ProcessFtp(List<FtpDownloader> downloaders)
        {
            log.Info("Press enter to stop monitoring...");
            var tasks = downloaders.Select(ftpDownloader => ftpDownloader.Download());
            var archiving = new DeleteArchiving();
            if (Archive.HasValue)
            {
                log.Info("Archiving...");
                archiving.Archive(Out, TimeSpan.FromDays(Archive.Value));
            }

            Task.WhenAll(tasks).Wait();
        }

        protected override IPredicate ConstructPredicate()
        {
            return new NullPredicate();
        }
    }
}
