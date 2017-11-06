namespace Wikiled.YiScanner.Monitoring
{
    public interface IMonitoringConfig : IScanConfig
    {
        int Scan { get; }
    }
}