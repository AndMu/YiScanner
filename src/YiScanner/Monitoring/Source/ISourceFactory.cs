using System;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public interface ISourceFactory
    {
        IObservable<IFtpDownloader> GetSources(IHostManager hosts);
    }
}