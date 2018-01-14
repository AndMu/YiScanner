using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;
using Wikiled.YiScanner.Monitoring;

namespace Wikiled.YiScanner.Commands
{
    public abstract class BaseCommand : Command, IScanConfig
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly FtpConfig ftpConfig;

        protected BaseCommand(FtpConfig ftpConfig)
        {
            Guard.NotNull(() => ftpConfig, ftpConfig);
            this.ftpConfig = ftpConfig;
        }

        public ActionConfig Action { get; }

        [Required]
        [Description("List of camera names")]
        public string Cameras { get; set; }

        [Required]
        [Description("List of camera hosts")]
        public string Hosts { get; set; }

        [Description("Compress video files")]
        public bool Compress { get; set; }

        [Description("Do you want to save it as image")]
        public bool Images { get; }

        [Required]
        [Description("File destination")]
        public string Out { get; set; }

        [Description("Archive video after days")]
        public int? Archive { get; set; }

        public override void Execute()
        {
            log.Info("Starting camera download...");
            DestinationFactory factory = new DestinationFactory(ftpConfig, this, ConstructPredicate());
            ProcessFtp(factory);
        }

        protected abstract IPredicate ConstructPredicate();

        protected abstract void ProcessFtp(IDestinationFactory downloaders);
    }
}
