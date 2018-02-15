using System;
using System.Threading.Tasks;

namespace Wikiled.YiScanner.Client
{
    public interface IFtpDownloader
    {
        Task<DateTime> Download();
    }
}