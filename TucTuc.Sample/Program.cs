using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TucTuc.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new DefaultConfiguration();

            var message = new DummyMessage { Text = "Hello" };

            string queue = Path.Combine(Path.GetTempPath(), "TucTuc");

            cfg.AddEndpoint(message.GetType(), queue);

            var bus = new Bus();

            bus.Start(cfg);

            bus.Send(message);
        }

        public class DummyMessage
        {
            public string Text;
        }
    }
}
