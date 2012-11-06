using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TucTuc
{
    public interface IConfiguration
    {
        string InputQueue { get; }
        ITransport Transport { get; }
        ISerializer Serializer { get; }

        void AddEndpoint(Type type, string endpoint);
        string GetEndpoint(Type type);
    }

    public class DefaultConfiguration : IConfiguration
    {
        private string _inputQueue;
        public string InputQueue
        {
            get
            {
                if (string.IsNullOrEmpty(_inputQueue))
                {
                    _inputQueue = Assembly.GetEntryAssembly().FullName;
                }
                return _inputQueue;
            }
            set { _inputQueue = value; }
        }

        private ITransport _transport;
        public ITransport Transport
        {
            get { return (_transport ?? (_transport = new FileTransport())); }
            set { _transport = value; }
        }

        private ISerializer _serializer;
        public ISerializer Serializer
        {
            get { return (_serializer ?? (_serializer = new JsonSerializer())); }
            set { _serializer = value; }
        }

        public IDictionary<Type, string> Endpoints { get; private set; }

        public DefaultConfiguration()
        {
            Endpoints = new Dictionary<Type, string>();
        }

        public string GetEndpoint(Type type)
        {
            string endpoint;
            if (!Endpoints.TryGetValue(type, out endpoint))
                return null;

            return endpoint;
        }

        public void AddEndpoint(Type type, string endpoint)
        {
            Endpoints.Add(type, endpoint);
        }
    }
}
