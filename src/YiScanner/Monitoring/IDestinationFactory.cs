using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Monitoring
{
    public interface IDestinationFactory
    {
        FtpDownloader[] GetDestinations();
    }
}