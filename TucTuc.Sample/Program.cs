using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TucTuc.Sample
{
    public class Program
    {
        static void Main(string[] args)
        {
            var message = new DummyMessage { Text = "Hello from Tuc Tuc!" };

            string queue = Path.Combine(Path.GetTempPath(), "TucTuc");

            var bus = new Bus(queue);
            bus.RegisterEndpoint<DummyMessage>(queue);
            bus.RegisterMessageHandler<DummyMessage>(m => Handle(m));
             
            bus.Start();

            while(true)
            {
                bus.Send(message);    

                Thread.Sleep(60000);
            }
        }

        static void Handle(DummyMessage msg)
        {
            Console.WriteLine("Message received on Thread: " + Thread.CurrentThread.ManagedThreadId + ", text: " + msg.Text);
        }
    }

    public class DummyMessage
    {
        public string Text { get; set; }
    }
}
