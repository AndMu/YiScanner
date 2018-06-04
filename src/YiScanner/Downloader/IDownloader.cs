using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wikiled.YiScanner.Downloader
{
    public interface IDownloader
    {
        Task<DateTime> Download(CancellationToken cancellation);
    }
}