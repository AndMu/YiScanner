using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public class CompressPriorAction : IPriorAction
    {
        public Task<(VideoHeader header, Stream source)> BeforeTransfer(VideoHeader header, Stream source)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => source, source);
            var memory = new MemoryStream();
            var zipStream = new ZipOutputStream(memory);
            zipStream.SetLevel(9);
            ZipEntry entry = new ZipEntry(Path.GetFileName(header.FileName));
            zipStream.PutNextEntry(entry);
            source.CopyTo(zipStream);
            zipStream.Flush();
            zipStream.Finish();
            memory.Position = 0;
            return Task.FromResult((header, (Stream)memory));
        }
    }
}
