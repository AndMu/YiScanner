using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Monitoring.Config
{
    public class MonitoringConfig
    {
        public int Scan { get; set; }

        public PredefinedCameraConfig Known { get; set; }

        public OutputConfig Output { get; set; }

        public int? Archive { get; set; }

        public ActionConfig Action { get; set; }

        public AutoDiscoveryConfig AutoDiscovery { get; set; }

        public FtpConfig YiFtp { get; set; }

        public ServerConfig Server { get; set; }
    }
}
