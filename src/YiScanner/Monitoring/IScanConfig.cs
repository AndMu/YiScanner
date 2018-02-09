using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Monitoring
{
    public interface IScanConfig
    {
        string Cameras { get; }

        string Hosts { get; }

        bool Compress { get; }

        bool Images { get; }

        string Out { get; }

        int? Archive { get; }

        ActionConfig Action { get; }

        bool? AutoDiscover { get; }

        string NetworkMask { get; }
    }
}