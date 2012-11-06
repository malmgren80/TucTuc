using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace TucTuc.Tests
{
    [TestFixture]
    public class BusTests
    {
        [Test]
        public void Send_Null_Throws()
        {
            var bus = new Bus();

            bus.Start();

            Assert.Throws<ArgumentNullException>(() => bus.Send<string>(null));
        }

        [Test]
        public void Send_NotConfigured_Throws()
        {
            var bus = new Bus();

            string someData = "some made up data";

            Assert.Throws<InvalidOperationException>(() => bus.Send(someData));
        }

        [Test]
        public void Send_NoEndpointForType_Throws()
        {
            var bus = new Bus();

            bus.Start();

            string someData = "some made up data";

            Assert.Throws<InvalidOperationException>(() => bus.Send(someData));
        }
    }
}
