using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Accord.Video.FFMPEG;
using Wikiled.Core.Utility.Arguments;
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
            var fileDestination = header.GetPath(destination);
            return File.Exists(fileDestination);
        }

        public async Task Transfer(VideoHeader header, Stream source)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => source, source);
            var fileDestination = header.GetPath(destination);
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
                    fileDestination = Path.ChangeExtension(fileDestination, "png");
                    using (Bitmap videoFrame = reader.ReadVideoFrame())
                    {
                        videoFrame.Save(fileDestination, ImageFormat.Png);
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
