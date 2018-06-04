using NLog;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Monitoring.Config
{
    public class MonitoringConfig
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public int Scan { get; set; }

        public int? TimeOut { get; set; }

        public PredefinedCameraConfig Known { get; set; }

        public OutputConfig Output { get; set; }

        public int? Archive { get; set; }

        public ActionConfig Action { get; set; }

        public AutoDiscoveryConfig AutoDiscovery { get; set; }

        public FtpConfig YiFtp { get; set; }

        public ServerConfig Server { get; set; }

        public bool Validate()
        {
            if (Output == null)
            {
                log.Error("Output is not defined");
                return false;
            }

            if (YiFtp == null)
            {
                log.Error("Yi Ftp is not defined");
                return false;
            }

            return true;
        }
    }
}
