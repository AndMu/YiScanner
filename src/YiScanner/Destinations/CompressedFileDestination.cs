using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Helpers;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public class CompressedDestination : IDestination
    {
        private IDestination another;

        public CompressedDestination(IDestination another)
        {
            Guard.NotNull(() => another, another);
            this.another = another;
        }

        public async Task Transfer(VideoHeader header, Stream source)
        {
            using (var memory = new MemoryStream())
            using (var zipStream = new GZipOutputStream(memory))
            {
                source.Position = 0;
                source.CopyTo(zipStream);
                zipStream.Flush();
                zipStream.Finish();
                memory.Position = 0;

                await another.Transfer(
                                 new VideoHeader(header.Camera, Path.ChangeExtension(header.FileName, "zip")),
                                 memory)
                             .ConfigureAwait(false);
            }
        }
    }
}
