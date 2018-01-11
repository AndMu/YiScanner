using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Concurrency;
using NLog;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Archive;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Monitoring;

namespace Wikiled.YiScanner.Commands
{
    /// <summary>
    ///     Monitor -Cameras=1080i -Hosts=192.168.0.202 [-Compress] -Out=c:\out -Scan=10 [-Archive=2]
    /// </summary>
    [Description("Monitor new video from cameras")]
    public class MonitorCommand : BaseCommand, IMonitoringConfig
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public MonitorCommand(FtpConfig ftpConfig)
            : base(ftpConfig)
        {
        }

        [Required]
        [Description("Scan interval in second")]
        public int Scan { get; set; }

        protected override IPredicate ConstructPredicate()
        {
            return new NewFilesPredicate();
        }

        protected override void ProcessFtp(IDestinationFactory downloaders)
        {
            var instance = new MonitoringInstance(TaskPoolScheduler.Default, this, downloaders, new DeleteArchiving());
            if (instance.Start())
            {
                log.Info("Press enter to stop monitoring...");
                Console.ReadLine();
                instance.Stop();
            }
        }
    }
}
