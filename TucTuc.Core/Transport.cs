using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using TucTuc.IO;

namespace TucTuc
{
    public interface ITransport
    {
        void Send(Guid id, string data, string endpoint);
        void StartListen(string inputQueue);

        event EventHandler<MessageReceivedEventArgs> OnMessageReceived;
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public string Data { get; set; }
    }

    public class FileTransport : ITransport
    {
        // TODO: Write the data in one file and a empty trigger file
 
        private const string Extension = "msg";

        private static string Pattern
        {
            get { return string.Format("*.{0}", Extension); }
        }

        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        public IFileSystem FileSystem { get; set; }
        public Encoding FileEncoding { get; set; }
        public IFileListener Listener { get; private set; }

        public FileTransport()
        {
            FileSystem = new FileSystem();
            FileEncoding = Encoding.Unicode;
        }

        public void Send(Guid id, string data, string endpoint)
        {
            var queue = new FileQueue(endpoint);

            FileSystem.EnsureExists(queue.FullPath);

            string fullPath = GetFilePath(queue, id);

            FileSystem.WriteFile(fullPath, data, FileEncoding);
        }

        private string GetOldestValidFile(string path, string pattern)
        {
            var files = FileSystem.GetFiles(path, pattern);

            // Get file names by creation date
            return 
                (from file in files
                orderby file.CreationTimeUtc
                select file.FullName).FirstOrDefault();
        }

        private string GetFilePath(FileQueue queue, Guid id)
        {
            string fileName = string.Format("{0}.{1}", id, Extension);
            return Path.Combine(queue.FullPath, fileName);
        }

        public void StartListen(string inputQueue)
        {
            Listener = new FileListener(inputQueue, Pattern);
            Listener.OnFileChanged += OnFileChanged;
            Listener.Start();
        }

        private void OnFileChanged(object sender, FileChangedEventArgs e)
        {
            // TODO: A new thread should be created here

            string file = GetOldestValidFile(e.Directory, Pattern);

            if (file != null)
            {
                var eventHandler = OnMessageReceived;
                if (eventHandler != null)
                {
                    var eventArgs = new MessageReceivedEventArgs
                    {
                        Data = FileSystem.ReadFile(file, FileEncoding),
                    };

                    eventHandler(this, eventArgs);
                }
            }
        }
    }

    public class FileQueue
    {
        public string FullPath
        {
            get { return Path.GetFullPath(_path); }
        }

        private readonly string _path;

        public FileQueue(string path)
        {
            _path = path;
        }
    }
}
