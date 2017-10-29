using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner
{
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Downloader");
            FtpDownloader downloader = new FtpDownloader("1080i", "192.168.0.103", new FileDestination("Test"));
            downloader.Download().Wait();
        }
    }
}
