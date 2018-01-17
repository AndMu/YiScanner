using System.Collections.Generic;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Monitoring
{
    public abstract class SourceFactoryBase : ISourceFactory
    {
        private readonly IPredicate filePredicate;

        private readonly FtpConfig ftpConfig;

        protected SourceFactoryBase(FtpConfig ftpConfig, IScanConfig config, IPredicate filePredicate)
        {
            Guard.NotNull(() => ftpConfig, ftpConfig);
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => filePredicate, filePredicate);
            this.ftpConfig = ftpConfig;
            Config = config;
            this.filePredicate = filePredicate;
        }

        public IScanConfig Config { get; }

        public abstract IEnumerable<IFtpDownloader> GetSources();

        protected IDestination ConstructDestination()
        {
            IDestination destination = Config.Images ? (IDestination)new PictureFileDestination(Config.Out) : new FileDestination(Config.Out);
            if (Config.Compress)
            {
                destination = ChainedPriorActionDestination.CreateCompressed(destination);
            }

            if (Config.Action != null)
            {
                destination = ChainedPostActionDestination.CreateAction(destination, Config.Action);
            }

            return destination;
        }

        protected IFtpDownloader ConstructDownloader(string cameraName, string host, IDestination destination)
        {
            return new FtpDownloader(
                ftpConfig,
                new CameraDescription(cameraName, host),
                destination,
                filePredicate);
        }
    }
}
