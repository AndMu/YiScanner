using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Console.Arguments;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Monitoring.Config;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Commands
{
    public abstract class BaseCommand : Command
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        protected BaseCommand(MonitoringConfig config)
        {
            Guard.NotNull(() => config, config);
            Config = config;
        }

        [Description("Archive video after days")]
        public int? Archive { get; set; }

        [Description("Auto discover cameras")]
        public bool? AutoDiscover { get; set; }

        [Required]
        [Description("List of camera names")]
        public string Cameras { get; set; }

        [Description("Compress video files")]
        public bool? Compress { get; set; }

        public MonitoringConfig Config { get; }

        [Required]
        [Description("List of camera hosts")]
        public string Hosts { get; set; }
        
        [Description("Do you want to save it as image")]
        public bool? Images { get; set; }

        [Description("Discovery network mask")]
        public string NetworkMask { get; set; }
        
        [Required]
        [Description("File destination")]
        public string Out { get; set; }
        
        public override void Execute()
        {
            log.Info("Starting camera download...");
            SetConfig();
            var factory = new SourceFactory(Config, ConstructPredicate());
            ProcessFtp(factory);
        }

        protected abstract IPredicate ConstructPredicate();

        protected abstract void ProcessFtp(ISourceFactory downloaders);

        private void SetConfig()
        {
            log.Info("Initializing config");
            Config.Archive = Archive;
            if (Config.AutoDiscovery == null)
            {
                Config.AutoDiscovery = new AutoDiscoveryConfig();
            }

            Config.AutoDiscovery.On = AutoDiscover;
            Config.AutoDiscovery.NetworkMask = NetworkMask;

            if (Config.Known == null)
            {
                Config.Known = new PredefinedCameraConfig();
            }

            Config.Known.Cameras = Cameras;
            Config.Known.Hosts = Hosts;

            if (Config.Output == null)
            {
                Config.Output = new OutputConfig();
            }

            Config.Output.Compress = Compress == true;
            Config.Output.Out = Out;
            Config.Output.Images = Images == true;

            // ignore FTP Server
            Config.Server = null;
        }
    }
}
