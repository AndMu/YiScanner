using System;
using System.IO;
using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public class ChainedPostActionDestination : ChainedPriorActionDestinationBase
    {
        private readonly IDestination next;

        private readonly IPostAction postAction;

        public ChainedPostActionDestination(IDestination next, IPostAction postAction)
            : base(next)
        {
            Guard.NotNull(() => next, next);
            Guard.NotNull(() => postAction, postAction);
            this.next = next;
            this.postAction = postAction;
        }

        public static ChainedPostActionDestination CreateAction(IDestination next, ActionConfig config)
        {
            Guard.NotNull(() => config, config);
            if (config.Type == ActionType.Execute)
            {
                return new ChainedPostActionDestination(next, new ExecutePostAction(config));
            }

            throw new NotImplementedException();
        }

        public override async Task Transfer(VideoHeader header, Stream source)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => source, source);
            var transfer = next.Transfer(header, source);
            var name = next.ResolveName(header);
            await transfer;
            await postAction.AfterTransfer(name);
        }
    }
}
