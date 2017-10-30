using Moq;
using NUnit.Framework;
using System;
using Wikiled.YiScanner.Client.Predicates;

namespace Wikiled.YiScanner.Tests.Client.Predicates
{
    [TestFixture]
    public class NewFilesPredicateTests
    {
        private NewFilesPredicate instance;

        [SetUp]
        public void Setup()
        {
            instance = CreateNewFilesPredicate();
        }

        [Test]
        public void CanDownload()
        {
            var result = instance.CanDownload(null, "Test.file", DateTime.Now);
            Assert.IsFalse(result);
            result = instance.CanDownload(DateTime.Now, "Test.file", DateTime.Now);
            Assert.IsFalse(result);
            result = instance.CanDownload(null, "Test.file", DateTime.Now);
            Assert.IsFalse(result);

            result = instance.CanDownload(DateTime.Now,  "Test2.file", DateTime.Now);
            Assert.IsTrue(result);
        }

        private NewFilesPredicate CreateNewFilesPredicate()
        {
            return new NewFilesPredicate();
        }
    }
}
