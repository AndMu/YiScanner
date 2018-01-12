using System.IO;
using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public abstract class ChainedPriorActionDestinationBase : IDestination
    {
        private readonly IDestination next;

        protected ChainedPriorActionDestinationBase(IDestination next)
        {
            Guard.NotNull(() => next, next);
            this.next = next;
        }

        public abstract Task Transfer(VideoHeader header, Stream source);

        public bool IsDownloaded(VideoHeader header)
        {
            Guard.NotNull(() => header, header);
            return next.IsDownloaded(header);
        }

        public string ResolveName(VideoHeader header)
        {
            Guard.NotNull(() => header, header);
            return next.ResolveName(header);
        }
    }
}