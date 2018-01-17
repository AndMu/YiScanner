using System.Collections.Generic;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Monitoring
{
    public interface ISourceFactory
    {
        IEnumerable<IFtpDownloader> GetSources();
    }
}