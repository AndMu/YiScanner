using System;
using System.Threading.Tasks;

namespace Wikiled.YiScanner.Client
{
    public interface IDownloader
    {
        Task<DateTime> Download();
    }
}