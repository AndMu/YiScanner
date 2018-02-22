using System;
using System.Linq;
using System.Net;
using Moq;
using NUnit.Framework;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Monitoring.Config;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Tests.Monitoring.Source
{
    [TestFixture]
    public class SourceFactoryTests
    {
        private MonitoringConfig scanConfig;

        private Mock<IPredicate> mockPredicate;

        private SourceFactory instance;

        private StaticHostManager manager;

        [SetUp]
        public void SetUp()
        {
            scanConfig = new MonitoringConfig();
            scanConfig.YiFtp = new FtpConfig();
            scanConfig.YiFtp.FileMask = "Test";
            scanConfig.Output = new OutputConfig();
            scanConfig.Output.Out = "Out";
            mockPredicate = new Mock<IPredicate>();
            scanConfig.Known = new PredefinedCameraConfig();
            manager = new StaticHostManager(scanConfig.Known);
            instance = CreateFactory();
        }

        [Test]
        public void GetSources()
        {
            var result = instance.GetSources(manager).ToArray();
            Assert.AreEqual(0, result.Length);

            scanConfig.Known.Cameras = "Test";
            result = instance.GetSources(manager).ToArray();
            Assert.AreEqual(0, result.Length);

            scanConfig.Known.Hosts = IPAddress.Any.ToString();
            result = instance.GetSources(manager).ToArray();
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new SourceFactory(null, mockPredicate.Object));
            Assert.Throws<ArgumentNullException>(() => new SourceFactory(scanConfig, null));
        }

        private SourceFactory CreateFactory()
        {
            return new SourceFactory(scanConfig, mockPredicate.Object);
        }
    }
}