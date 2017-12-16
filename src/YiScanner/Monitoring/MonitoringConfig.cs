namespace Wikiled.YiScanner.Monitoring
{
    public class MonitoringConfig : IMonitoringConfig
    {
        public int Scan { get; set; }

        public bool All { get; set; }

        public string Cameras { get; set; }

        public string Hosts { get; set; }

        public bool Compress { get; set; }

        public bool Images { get; set; }

        public string Out { get; set; }

        public int? Archive { get; set; }
    }
}
