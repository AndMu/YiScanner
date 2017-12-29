using System;
using System.IO;
using System.Threading.Tasks;
using NLog;

namespace Wikiled.YiScanner.Client.Archive
{
    public class DeleteArchiving : IDeleteArchiving
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public Task Archive(string destination, TimeSpan time)
        {
            return Task.Run(() => ArchiveInternal(destination, time));
        }

        private void ArchiveInternal(string destination, TimeSpan time)
        {
            if (!Directory.Exists(destination))
            {
                return;
            }

            var files = Directory.EnumerateFiles(destination, "*", SearchOption.AllDirectories);
            DateTime cutOff = DateTime.Today.Subtract(time);
            Parallel.ForEach(
                files,
                file =>
                    {
                        try
                        {
                            var info = new FileInfo(file);
                            if (info.CreationTime < cutOff)
                            {
                                log.Debug("Deleting: {0}", file);
                                File.Delete(file);
                            }
                        }
                        catch(Exception ex)
                        {
                            log.Error(ex);
                        }
                    });
        }
    }
}
