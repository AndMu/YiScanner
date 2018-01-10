using System.IO;
using System.Threading.Tasks;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public interface IDestination
    {
        Task Transfer(VideoHeader header, Stream source);

        bool IsDownloaded(VideoHeader header);

        string ResolveName(VideoHeader header);
    }
}