using System;
using System.Linq;
using System.Net;
using Moq;
using NUnit.Framework;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Monitoring;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Tests.Monitoring.Source
{
    [TestFixture]
    public class SourceFactoryTests
    {
        private FtpConfig ftpConfig;

        private MonitoringConfig scanConfig;

        private Mock<IPredicate> mockPredicate;

        private SourceFactory instance;

        private StaticHostManager manager;

        [SetUp]
        public void SetUp()
        {
            ftpConfig = new FtpConfig();
            ftpConfig.FileMask = "Test";
            scanConfig = new MonitoringConfig();
            scanConfig.Out = "Out";
            mockPredicate = new Mock<IPredicate>();
            manager = new StaticHostManager(scanConfig);
            instance = CreateFactory();
        }

        [Test]
        public void GetSources()
        {
            var result = instance.GetSources(manager).ToArray();
            Assert.AreEqual(0, result.Length);

            scanConfig.Cameras = "Test";
            result = instance.GetSources(manager).ToArray();
            Assert.AreEqual(0, result.Length);

            scanConfig.Hosts = IPAddress.Any.ToString();
            result = instance.GetSources(manager).ToArray();
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new SourceFactory(
                null,
                scanConfig,
                mockPredicate.Object));

            Assert.Throws<ArgumentNullException>(() => new SourceFactory(
                ftpConfig,
                null,
                mockPredicate.Object));

            Assert.Throws<ArgumentNullException>(() => new SourceFactory(
                ftpConfig,
                scanConfig,
                null));
        }

        private SourceFactory CreateFactory()
        {
            return new SourceFactory(
                ftpConfig,
                scanConfig,
                mockPredicate.Object);
        }
    }
}