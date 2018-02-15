using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Archive;
using Wikiled.YiScanner.Monitoring;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Tests.Monitoring
{
    [TestFixture]
    public class MonitoringInstanceTests : ReactiveTest
    {
        private MonitoringConfig monitoringConfig;

        private Mock<ISourceFactory> mockDestinationFactory;

        private Mock<IFtpDownloader> ftpDownloader;

        private Mock<IDeleteArchiving> archiving;

        private MonitoringInstance instance;

        private TestScheduler scheduler;

        [SetUp]
        public void SetUp()
        {
            scheduler = new TestScheduler();
            monitoringConfig = new MonitoringConfig();
            monitoringConfig.Scan = 1;
            archiving = new Mock<IDeleteArchiving>();
            mockDestinationFactory = new Mock<ISourceFactory>();
            ftpDownloader = new Mock<IFtpDownloader>();
            
            var isExecuting = scheduler.CreateColdObservable(
                new Recorded<Notification<IFtpDownloader>>(0, Notification.CreateOnNext(ftpDownloader.Object)),
                new Recorded<Notification<IFtpDownloader>>(0, Notification.CreateOnCompleted<IFtpDownloader>()));

            mockDestinationFactory.Setup(item => item.GetSources(It.IsAny<IHostManager>()))
                                  .Returns(isExecuting);
            instance = CreateMonitoringInstance();
        }

        [Test]
        public void Download()
        {
            instance.Start();
            scheduler.AdvanceBy(TimeSpan.FromSeconds(11).Ticks);
            ftpDownloader.Verify(item => item.Download(), Times.Exactly(10));
        }

        [Test]
        public void DownloadSlow()
        {
            ftpDownloader.Setup(item => item.Download()).Returns(() => Observable.Return(DateTime.Now).Delay(TimeSpan.FromSeconds(1), scheduler)
                                                                                 .Select(
                                                                                     item =>
                                                                                         {
                                                                                             var now = scheduler.Now;
                                                                                             return item;
                                                                                         })
                                                                                 .ToTask());
            instance.Start();
            for (int i = 0; i < 10; i++)
            {
                scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
            }
            
            ftpDownloader.Verify(item => item.Download(), Times.Exactly(2));
        }

        [Test]
        public void Archive()
        {
            monitoringConfig.Archive = 2;
            monitoringConfig.Scan = int.MaxValue;
            archiving.Setup(item => item.Archive(It.IsAny<string>(), It.IsAny<TimeSpan>())).Returns(() => Observable.Return(true).Delay(TimeSpan.FromSeconds(5), scheduler).ToTask());
            instance.Start();
            scheduler.AdvanceBy(TimeSpan.FromDays(3).Ticks);
            archiving.Verify(item => item.Archive(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Exactly(2));
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new MonitoringInstance(
                null,
                monitoringConfig,
                mockDestinationFactory.Object,
                archiving.Object));

            Assert.Throws<ArgumentNullException>(() => new MonitoringInstance(
                scheduler,
                null,
                mockDestinationFactory.Object,
                archiving.Object));

            Assert.Throws<ArgumentNullException>(() => new MonitoringInstance(
                scheduler,
                monitoringConfig,
                null,
                archiving.Object));

            Assert.Throws<ArgumentNullException>(() => new MonitoringInstance(
                scheduler,
                monitoringConfig,
                mockDestinationFactory.Object,
                null));
        }

        private MonitoringInstance CreateMonitoringInstance()
        {
            return new MonitoringInstance(
                scheduler,
                monitoringConfig,
                mockDestinationFactory.Object,
                archiving.Object);
        }
    }
}