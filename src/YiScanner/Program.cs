using System.Net;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner
{
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Processing cameras...");
            FtpDownloader downloader = new FtpDownloader(new CameraDescription("1080i", IPAddress.Parse("192.168.0.103")), new FileDestination("Test"));
            downloader.Download().Wait();
        }
    }
}
