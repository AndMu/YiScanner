using System;
using System.IO;
using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public class TransformedDestination : IDestination
    {
        private readonly IDestination another;

        private readonly Func<string, string> nameChange;

        public TransformedDestination(IDestination another, Func<string, string> nameChange)
        {
            Guard.NotNull(() => another, another);
            Guard.NotNull(() => nameChange, nameChange);
            this.another = another;
            this.nameChange = nameChange;
        }

        public static TransformedDestination ChangeExtension(IDestination another, string nenExtension)
        {
            return new TransformedDestination(another, fileName => Path.ChangeExtension(fileName, nenExtension));
        }

        public Task Transfer(VideoHeader header, Stream source)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => source, source);
            return another.Transfer(ConstructHeader(header), source);
        }

        public bool IsDownloaded(VideoHeader header)
        {
            Guard.NotNull(() => header, header);
            return another.IsDownloaded(ConstructHeader(header));
        }

        public string ResolveName(VideoHeader header)
        {
            Guard.NotNull(() => header, header);
            return another.ResolveName(ConstructHeader(header));
        }

        private VideoHeader ConstructHeader(VideoHeader header)
        {
            return new VideoHeader(header.Camera, nameChange(header.FileName));
        }
    }
}
