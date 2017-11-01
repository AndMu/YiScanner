using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Archive;
using Wikiled.YiScanner.Client.Predicates;

namespace Wikiled.YiScanner.Commands
{
    /// <summary>
    ///     Monitor -Cameras=1080i -Hosts=192.168.0.202 [-Compress] -Out=c:\out -Scan=10 [-Archive=2]
    /// </summary>
    [Description("Monitor new video from cameras")]
    public class MonitorCommand : BaseCommand
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public MonitorCommand(FtpConfiguration ftpConfiguration)
            : base(ftpConfiguration)
        {
        }

        [Required]
        [Description("Scan interval in second")]
        public int Scan { get; set; }

        protected override IPredicate ConstructPredicate()
        {
            return new NewFilesPredicate();
        }

        protected override void ProcessFtp(List<FtpDownloader> downloaders)
        {
            log.Info("Press enter to stop monitoring...");
            if (Archive.HasValue)
            {
                var archiving = Observable.Interval(TimeSpan.FromDays(1), TaskPoolScheduler.Default)
                                          .Select(item => Archiving())
                                          .StartWith(Archiving())
                                          .Replay();
                archiving.Connect();
            }

            var observable = Observable.Interval(TimeSpan.FromSeconds(Scan), TaskPoolScheduler.Default)
                                       .Select(item => Download(downloaders))
                                       .StartWith(Download(downloaders))
                                       .Replay();
            observable.Connect();

            Console.ReadLine();
        }

        private bool Archiving()
        {
            var archiving = new DeleteArchiving();
            log.Info("Archiving...");
            archiving.Archive(Out, TimeSpan.FromDays(Archive.Value));
            log.Info("Archiving. Done!");
            return true;
        }

        private async Task Download(List<FtpDownloader> downloaders)
        {
            log.Debug("Checking Ftp....");
            try
            {
                var tasks = downloaders.Select(ftpDownloader => ftpDownloader.Download());
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            log.Debug("Done!");
        }
    }
}
