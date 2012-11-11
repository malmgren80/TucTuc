using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FakeItEasy;
using NUnit.Framework;

namespace TucTuc.Tests
{
    [TestFixture]
    public class BusTests
    {
        [Test]
        public void Send_Null_Throws()
        {
            var transport = A.Fake<ITransport>();

            var bus = new Bus("myQueue");

            bus.Transport = transport;

            bus.Start();

            Assert.Throws<ArgumentNullException>(() => bus.Send<string>(null));
        }

        [Test]
        public void Send_NotConfigured_Throws()
        {
            var transport = A.Fake<ITransport>();

            var bus = new Bus("myQueue");

            bus.Transport = transport;

            string someData = "some made up data";

            Assert.Throws<InvalidOperationException>(() => bus.Send(someData));
        }

        [Test]
        public void Send_NoEndpointForType_Throws()
        {
            var transport = A.Fake<ITransport>();

            var bus = new Bus("myQueue");

            bus.Transport = transport;

            bus.Start();

            string someData = "some made up data";

            Assert.Throws<InvalidOperationException>(() => bus.Send(someData));
        }

        [Test]
        public void Start_InputQueueSet_RegisterTransportListener()
        {
            string queueName = "myQueue";

            var transport = A.Fake<ITransport>();
            var bus = new Bus(queueName) { Transport = transport };

            bus.Start();

            A.CallTo(() => transport.StartListen(queueName)).MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}
