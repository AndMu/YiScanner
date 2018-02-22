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
        public int? Archive { get => Config.Archive; set => Config.Archive = value; }

        [Description("Auto discover cameras")]
        public bool? AutoDiscover
        {
            get => Config.AutoDiscovery?.On;
            set
            {
                if (Config.AutoDiscovery == null)
                {
                    Config.AutoDiscovery = new AutoDiscoveryConfig();
                }

                Config.AutoDiscovery.On = value;
            }
        }

        [Required]
        [Description("List of camera names")]
        public string Cameras
        {
            get => Config.Known?.Cameras;
            set
            {
                if (Config.Known == null)
                {
                    Config.Known = new PredefinedCameraConfig();
                }

                Config.Known.Cameras = value;
            }
        }

        [Description("Compress video files")]
        public bool? Compress
        {
            get => Config.Output?.Compress;
            set
            {
                if (Config.Output == null)
                {
                    Config.Output = new OutputConfig();
                }

                Config.Output.Compress = value.Value;
            }
        }

        public MonitoringConfig Config { get; }

        [Required]
        [Description("List of camera hosts")]
        public string Hosts
        {
            get => Config.Known?.Hosts;
            set
            {
                if (Config.Known == null)
                {
                    Config.Known = new PredefinedCameraConfig();
                }

                Config.Known.Hosts = value;
            }
        }

        [Description("Do you want to save it as image")]
        public bool? Images
        {
            get => Config.Output?.Images;
            set
            {
                if (Config.Output == null)
                {
                    Config.Output = new OutputConfig();
                }

                Config.Output.Images = value.Value;
            }
        }

        [Description("Discovery network mask")]
        public string NetworkMask
        {
            get => Config.AutoDiscovery?.NetworkMask;
            set
            {
                if (Config.AutoDiscovery == null)
                {
                    Config.AutoDiscovery = new AutoDiscoveryConfig();
                }

                Config.AutoDiscovery.NetworkMask = value;
            }
        }

        [Required]
        [Description("File destination")]
        public string Out
        {
            get => Config.Output?.Out;
            set
            {
                if (Config.Output == null)
                {
                    Config.Output = new OutputConfig();
                }

                Config.Output.Out = value;
            }
        }

        public override void Execute()
        {
            log.Info("Starting camera download...");
            SourceFactory factory = new SourceFactory(Config, ConstructPredicate());
            ProcessFtp(factory);
        }

        protected abstract IPredicate ConstructPredicate();

        protected abstract void ProcessFtp(ISourceFactory downloaders);
    }
}
