using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TucTuc.Tests
{
    [TestFixture]
    public class DefaultConfigurationTests
    {
        [Test]
        public void DefaultConfiguration_NewInstance_SetsTransport()
        {
            var cfg = new Bus("myQueue");

            Assert.That(cfg.Transport, Is.Not.Null);
        }

        [Test]
        public void DefaultConfiguration_NewInstance_SetsSerializer()
        {
            var cfg = new Bus("myQueue");

            Assert.That(cfg.Serializer, Is.Not.Null);
        }
    }
}
