using System;
using System.IO;
using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public class ChainedActionDestination : IDestination
    {
        private IDestination next;

        public ChainedActionDestination(IDestination next)
        {
            Guard.NotNull(() => next, next);
            this.next = next;
        }

        public Task Transfer(VideoHeader header, Stream source)
        {
            throw new NotImplementedException();
        }

        public bool IsDownloaded(VideoHeader header)
        {
            throw new NotImplementedException();
        }

        public string ResolveName(VideoHeader header)
        {
            throw new NotImplementedException();
        }
    }
}
