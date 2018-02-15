using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Wikiled.Common.Arguments;
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
            ZipArchive archive = new ZipArchive(memory, ZipArchiveMode.Create, true);
            ZipArchiveEntry readmeEntry = archive.CreateEntry(Path.GetFileName(header.FileName));

            using (Stream entryStream = readmeEntry.Open())
            {
                source.CopyTo(entryStream);
            }
            
            memory.Position = 0;
            return Task.FromResult((header, (Stream)memory));
        }
    }
}
