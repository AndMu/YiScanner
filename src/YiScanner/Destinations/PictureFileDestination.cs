using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Accord.Video.FFMPEG;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public class PictureFileDestination : IDestination
    {
        private readonly string destination;

        public PictureFileDestination(string destination)
        {
            Guard.NotNullOrEmpty(() => destination, destination);
            this.destination = destination;
        }

        public bool IsDownloaded(VideoHeader header)
        {
            Guard.NotNull(() => header, header);
            return File.Exists(ResolveName(header));
        }

        public string ResolveName(VideoHeader header)
        {
            var fileDestination = header.GetPath(destination);
            return Path.ChangeExtension(fileDestination, "png");
        }

        public async Task Transfer(VideoHeader header, Stream source)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => source, source);

            var temp = Path.GetTempFileName();
            using (StreamWriter write = new StreamWriter(temp))
            {
                await source.CopyToAsync(write.BaseStream).ConfigureAwait(false);
            }

            try
            {
                using (VideoFileReader reader = new VideoFileReader())
                {
                    reader.Open(temp);
                    using (Bitmap videoFrame = reader.ReadVideoFrame())
                    {
                        videoFrame.Save(ResolveName(header), ImageFormat.Png);
                    }
                }
            }
            finally
            {
                File.Delete(temp);
            }
        }
    }
}
