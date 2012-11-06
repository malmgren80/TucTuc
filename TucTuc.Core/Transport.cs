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
        void MessageReceived(string inputQueue);

        event EventHandler<MessageReceivedEventArgs> OnMessageReceived;
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public string Queue { get; set; }
        public Guid Id { get; set; }
    }

    public class FileTransport : ITransport
    {
        // TODO: Write the data in one file and a command in another (empty) file
 
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

        public void MessageReceived(string inputQueue)
        {
            // TODO: A new thread should be created here

            string file = GetOldestValidFile(inputQueue, Pattern);

            if (file != null)
            {
                var eventHandler = OnMessageReceived;
                if (eventHandler != null)
                {
                    var eventArgs = new MessageReceivedEventArgs
                    {
                        Id = Guid.Parse(file),
                        Queue = inputQueue,
                    };

                    eventHandler(this, eventArgs);
                }
            }
        }

        private string GetOldestValidFile(string path, string pattern)
        {
            var files = FileSystem.GetFiles(path, pattern);

            // Get file names by creation date
            var orderedFileNames =
                from file in files
                orderby file.CreationTimeUtc
                select Path.GetFileNameWithoutExtension(file.Name);
            
            Guid guid;

            return
                (from fileName in orderedFileNames
                where Guid.TryParse(fileName, out guid)
                select fileName).FirstOrDefault();
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
            MessageReceived(e.Directory);
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
