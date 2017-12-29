using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Monitoring
{
    public interface IDestinationFactory
    {
        IFtpDownloader[] GetDestinations();
    }
}