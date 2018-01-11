using System.IO;
using System.Threading.Tasks;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public interface IPriorAction
    {
        Task<(VideoHeader header, Stream source)> BeforeTransfer(VideoHeader header, Stream source);
    }
}
