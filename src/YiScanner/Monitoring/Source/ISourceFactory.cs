using System.Collections.Generic;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public interface ISourceFactory
    {
        IEnumerable<IDownloader> GetSources(IHostManager hosts);
    }
}