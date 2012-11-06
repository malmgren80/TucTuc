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

            Config.Transport.Message(serializedData, endpoint);
        }
    }
}
