using System.IO;
using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public class ChainedPriorActionDestination : IDestination
    {
        private readonly IDestination next;

        private readonly IPriorAction priorAction;

        public ChainedPriorActionDestination(IDestination next, IPriorAction priorAction)
        {
            Guard.NotNull(() => next, next);
            Guard.NotNull(() => priorAction, priorAction);
            this.next = next;
            this.priorAction = priorAction;
        }

        public static IDestination CreateCompressed(IDestination next)
        {
            return TransformedDestination.ChangeExtension(
                new ChainedPriorActionDestination(next, new CompressPriorAction()),
                "zip");
        }

        public async Task Transfer(VideoHeader header, Stream source)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => source, source);
            var result = await priorAction.BeforeTransfer(header, source);
            using (result.source)
            {
                await next.Transfer(result.header, result.source);
            }
        }

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
