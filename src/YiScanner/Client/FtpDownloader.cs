using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentFTP;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Client
{
    public class FtpDownloader
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly CameraDescription camera;

        private readonly FtpConfiguration configuration;

        private readonly IDestination destination;

        private readonly Regex maskRegex;

        private readonly IPredicate predicate;

        private DateTime? lastScan;

        public FtpDownloader(
            FtpConfiguration configuration,
            CameraDescription camera,
            IDestination destination,
            IPredicate predicate)
        {
            Guard.NotNull(() => configuration, configuration);
            Guard.NotNull(() => camera, camera);
            Guard.NotNull(() => destination, destination);
            Guard.NotNull(() => predicate, predicate);
            maskRegex = FileMask.GenerateFitMask(configuration.FileMask);
            this.camera = camera;
            this.destination = destination;
            this.predicate = predicate;
            this.configuration = configuration;
        }

        public async Task Download()
        {
            // Get the object used to communicate with the server.  
            using (var client = new FtpClient(camera.Address))
            {
                log.Info("Connecting: {0}", camera.Address);
                client.Credentials = new NetworkCredential(
                    configuration.Login,
                    configuration.Password);
                client.Connect();
                log.Info("Connected: {0}!", camera.Address);
                await Retrieve(client, configuration.Path).ConfigureAwait(false);
            }

            lastScan = DateTime.Now;
        }

        private async Task ProcessFile(FtpClient client, FtpListItem item)
        {
            Stream stream = null;
            try
            {
                var header = new VideoHeader(camera, item.FullName);
                if (!destination.IsDownloaded(header))
                {
                    log.Info("Downloading <{0}> from [{1}]", item.FullName, camera.Name);
                    stream = await client.OpenReadAsync(item.FullName).ConfigureAwait(false);
                    await destination.Transfer(header, stream).ConfigureAwait(false);
                    var reply = await client.GetReplyAsync().ConfigureAwait(false);
                    if (reply.Success)
                    {
                        log.Info(
                            "Download Success:{0} Message:{1}: Type:{2} Code:{3} From: [{4}]",
                            reply.Success,
                            reply.Message,
                            reply.Type,
                            reply.Code,
                            camera.Name);
                    }
                    else
                    {
                        log.Error(
                            "Download Error:{0} Type:{1}: Code:{2} From: [{3}]",
                            reply.ErrorMessage,
                            reply.Type,
                            reply.Code,
                            camera.Name);
                    }

                    stream = null;
                }
                else
                {
                    log.Info("File is already downloaded - <{0}> {1}", item.FullName, camera.Name);
                }
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                stream?.Dispose();
            }
        }

        private async Task Retrieve(FtpClient client, string path)
        {
            foreach (FtpListItem item in client.GetListing(path))
            {
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    if (maskRegex.IsMatch(item.FullName) &&
                        predicate.CanDownload(lastScan, item.FullName, item.Modified))
                    {
                        if (item.Modified < DateTime.Now.AddMinutes(1))
                        {
                            await ProcessFile(client, item).ConfigureAwait(false);
                        }
                        else
                        {
                            log.Debug("Ignoring file - too recent: <{0}>", item.FullName);
                        }
                    }
                    else
                    {
                        log.Debug("Ignoring file: <{0}>", item.FullName);
                    }
                }
                else if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    await Retrieve(client, item.FullName).ConfigureAwait(false);
                }
            }
        }
    }
}
