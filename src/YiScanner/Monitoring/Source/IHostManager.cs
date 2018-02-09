using System;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public interface IHostManager : IDisposable
    {
        IObservable<FtpHost> GetHosts();
    }
}