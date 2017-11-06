namespace Wikiled.YiScanner.Monitoring
{
    public interface IScanConfig
    {
        string Cameras { get; }

        string Hosts { get; }

        bool Compress { get; }

        string Out { get; }

        int? Archive { get; }
    }
}