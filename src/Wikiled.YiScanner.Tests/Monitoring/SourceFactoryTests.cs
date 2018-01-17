using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Monitoring;

namespace Wikiled.YiScanner.Tests.Monitoring
{
    [TestFixture]
    public class SourceFactoryTests
    {
        private FtpConfig ftpConfig;

        private MonitoringConfig scanConfig;

        private Mock<IPredicate> mockPredicate;

        private SourceFactory instance;

        [SetUp]
        public void SetUp()
        {
            ftpConfig = new FtpConfig();
            ftpConfig.FileMask = "Test";
            scanConfig = new MonitoringConfig();
            scanConfig.Out = "Out";
            mockPredicate = new Mock<IPredicate>();
            instance = CreateFactory();
        }

        [Test]
        public void GetSources()
        {
            var result = instance.GetSources().ToArray();
            Assert.AreEqual(0, result.Length);

            scanConfig.Cameras = "Test";
            result = instance.GetSources().ToArray();
            Assert.AreEqual(0, result.Length);

            scanConfig.Hosts = "Test";
            result = instance.GetSources().ToArray();
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