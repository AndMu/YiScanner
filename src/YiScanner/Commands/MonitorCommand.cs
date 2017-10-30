using System;
using System.Collections.Generic;
using NLog;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Archive;
using Wikiled.YiScanner.Client.Predicates;

namespace Wikiled.YiScanner.Commands
{
    /// <summary>
    /// Monitor -Cameras=1080i -Hosts=192.168.0.202 -Compress -Out=c:\out -Scan=10
    /// </summary>
    [Description("Monitor new video from cameras")]
    public class MonitorCommand : BaseCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [Required]
        [Description("Scan interval in second")]
        public int Scan { get; set; }

        private async Task Download(List<FtpDownloader> downloaders)
        {
            log.Info("Starting download....");
            try
            {
                var tasks = downloaders.Select(ftpDownloader => ftpDownloader.Download());
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected override void ProcessFtp(List<FtpDownloader> downloaders)
        {
            log.Info("Press enter to stop monitoring...");
            var observable = Observable.Interval(TimeSpan.FromSeconds(Scan))
                                       .Select(item => Download(downloaders))
                                       .Replay();
            observable.Connect();

            if (Archive.HasValue)
            {
                var archiving = Observable.Interval(TimeSpan.FromDays(1))
                                       .Select(item => Archiving())
                                       .Replay();
                archiving.Connect();
            }

            Console.ReadLine();
        }

        protected override IPredicate ConstructPredicate()
        {
            return new NewFilesPredicate();
        }

        private bool Archiving()
        {
            var archiving = new DeleteArchiving();
            log.Info("Archiving...");
            archiving.Archive(Out, TimeSpan.FromDays(Archive.Value));
            return true;
        }
    }
}
