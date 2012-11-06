using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TucTuc.IO;

namespace TucTuc.Tests
{
    [TestFixture]
    public class FileTransportTests
    {
        [Test]
        public void StartListen_Queue_CreatesListener()
        {
            var transport = new FileTransport();

            transport.StartListen(Path.GetTempPath());

            Assert.That(((FileListener)transport.Listener).IsEnabled);
        }
    }
}
