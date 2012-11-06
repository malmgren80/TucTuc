using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TucTuc
{
    public interface IBus
    {
        void Start(IConfiguration configuration);
        void Send<T>(T message);
    }

    public class Bus : IBus
    {
        public IConfiguration Config { get; private set; }

        public void Start(IConfiguration configuration = null)
        {
            Config = configuration ?? new DefaultConfiguration();

            Config.Transport.StartListen(Config.InputQueue);

            Config.Transport.OnMessageReceived += OnMessageReceived;
            
            /* TODO:
             * Handlers should be registered at startup in Bus by this procedure:
             * 1. Scan all assemblies found for IMessageHandler<T>
             * 2. Register every messagehandler in a dictionary where T is key and messagehandler type is value
             * 3. It should also be possible to register factory method Func<T, IMessageHandler<T>> in dictionary
             */
        }

        public void Send<T>(T data)
        {
            if (data == null) throw new ArgumentNullException("data", "data can not be null");
            if (Config == null) throw new InvalidOperationException("Bus is not started, make sure Start() is called before sending messages.");

            string endpoint = Config.GetEndpoint(data.GetType());
            if (string.IsNullOrEmpty(endpoint))
                throw new InvalidOperationException(string.Format("No endpoint configured for message type: {0}", data.GetType()));

            var message = new TucTucMessage<T>
            {
                Id = Guid.NewGuid(),
                SentAtUtc = DateTime.UtcNow,
                Payload = data,
            };

            string serializedData = Config.Serializer.Serialize(message);

            Config.Transport.Send(message.Id, serializedData, endpoint);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            /*
             * 
             * When bus is notifed about new message and at startup of bus:
             * 1. If all workerthreads are busy, exit
             * 2. Find handler for message
             * 3. No MessageHandler found -> move to error queue
             * 4. Instantiate message handler:
             *    a. Try to find factory method
             *    b. Try to find default constructor
             * 5. If instantiate message handler failed -> move to error queue
             * 6. Create new worker thread from pool
             * 7. Check for new messages -> Jump to 1 or exit.
             * 
             * Message thread is processing like this:
             * 1. Check if there are any handlers plugged into the pipeline -> call OnProcessing
             * 2. Let thread process message
             * 3. Check if there are any handlers plugged into the pipeline -> call OnProcessed
             * 4. CleanUp of resources ? 
             */
        }
    }
}
