using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace TucTuc
{
    public interface IBus
    {
        ITransport Transport { get; set; }
        ISerializer Serializer { get; set; }

        void Start();
        void Send<T>(T message);
    }

    public class Bus : IBus, IConfiguration
    {
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

        private readonly string _inputQueue;
        private readonly ConcurrentDictionary<Type, string> _endpoints;
        private readonly ConcurrentDictionary<Type, object> _messageHandlers;

        public Bus(string inputQueue)
        {
            if (string.IsNullOrWhiteSpace(inputQueue)) throw new ArgumentException("inputQueue must have a value");

            _inputQueue = inputQueue;

            _endpoints = new ConcurrentDictionary<Type, string>();
            _messageHandlers = new ConcurrentDictionary<Type, object>();
        }

        public Bus() : this(Assembly.GetEntryAssembly().FullName)
        {
        }

        public void Start()
        {
            Transport.StartListen(_inputQueue);
            Transport.OnMessageReceived += OnMessageReceived;

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

            string endpoint = GetEndpoint<T>();
            if (string.IsNullOrEmpty(endpoint))
                throw new InvalidOperationException(string.Format("No endpoint configured for message type: {0}", data.GetType()));

            var message = new TucTucMessage
            {
                Id = Guid.NewGuid(),
                SentAtUtc = DateTime.UtcNow,
                Sender = _inputQueue,
                Payload = data,
            };

            string serializedData = Serializer.Serialize(message);

            Transport.Send(message.Id, serializedData, endpoint);

            Console.WriteLine("Message sent by Thread: " + Thread.CurrentThread.ManagedThreadId);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var message = Serializer.Deserialize<TucTucMessage>(e.Data);

            // 2. Find handler for message
            var handler = GetMessageHandler(message.Payload.GetType());

            if (handler == null)
            {
                throw new InvalidOperationException(string.Format("No handler found for message type: {0}", message.Payload));
            }

            handler(message.Payload);


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

        private string GetEndpoint<T>()
        {
            string endpoint;
            if (!_endpoints.TryGetValue(typeof(T), out endpoint))
                return null;

            return endpoint;
        }

        public void RegisterEndpoint<T>(string endpoint)
        {
            _endpoints[typeof(T)] = endpoint;
        }

        public void RegisterMessageHandler<T>(Action<T> handler)
        {
            _messageHandlers[typeof (T)] = CastArgument<object, T>(x => handler(x));
        }

        private static Action<TBase> CastArgument<TBase, TDerived>(Expression<Action<TDerived>> source) where TDerived : TBase
        {
            if (typeof(TDerived) == typeof(TBase))
                return (Action<TBase>)((Delegate)source.Compile());
            var sourceParameter = Expression.Parameter(typeof(TBase), "source");
            var result = Expression.Lambda<Action<TBase>>(Expression.Invoke(source, Expression.Convert(sourceParameter, typeof(TDerived))), sourceParameter);
            return result.Compile();
        }

        private Action<object> GetMessageHandler(Type type)
        {
            object handler;
            if (!_messageHandlers.TryGetValue(type, out handler))
                return null;

            return handler as Action<object>;
        }
    }
}
