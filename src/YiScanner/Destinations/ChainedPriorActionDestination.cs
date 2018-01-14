using System.IO;
using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public class ChainedPriorActionDestination : ChainedPriorActionDestinationBase
    {
        private readonly IPriorAction priorAction;

        private readonly IDestination next;

        public ChainedPriorActionDestination(IDestination next, IPriorAction priorAction)
        : base(next)
        {
            Guard.NotNull(() => next, next);
            Guard.NotNull(() => priorAction, priorAction);
            this.next = next;
            this.priorAction = priorAction;
        }

        public static IDestination CreateCompressed(IDestination next)
        {
            return TransformedDestination.ChangeExtension(new ChainedPriorActionDestination(next, new CompressPriorAction()), "zip");
        }

        public override async Task Transfer(VideoHeader header, Stream source)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => source, source);
            var result = await priorAction.BeforeTransfer(header, source).ConfigureAwait(false);
            using (result.source)
            {
                await next.Transfer(result.header, result.source).ConfigureAwait(false);
            }
        }
    }
}
