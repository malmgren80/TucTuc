using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TucTuc.Tests
{
    [TestFixture]
    public class JsonSerializerTests
    {
        [Test]
        public void Serialize_ValidMessage_ReturnsString()
        {
            var serializer = new JsonSerializer();

            var msg = new TestMessage { Text = "TucTuc honks the horn" };

            string data = serializer.Serialize(msg);

            Assert.That(data, Is.Not.Null);
            Assert.That(data.Length, Is.GreaterThan(0));
        }

        [Test]
        public void Deserialize_ValidData_ReturnsInstance()
        {
            var serializer = new JsonSerializer();

            string data = "{\"Text\":\"TucTuc honks the horn\"}";

            var msg = serializer.Deserialize<TestMessage>(data);

            Assert.That(msg, Is.Not.Null);
            Assert.That(msg.Text.Equals("TucTuc honks the horn"));
        }

        public class TestMessage
        {
            public string Text;
        }
    }
}
