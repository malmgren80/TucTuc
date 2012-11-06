using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TucTuc.IO
{
    public interface IFileListener : IDisposable
    {
        event EventHandler<FileChangedEventArgs> OnFileChanged;
        bool IsEnabled { get; }

        void Start();
        void Stop();
    }

    public class FileListener : IFileListener
    {
        private readonly FileSystemWatcher _watcher;

        public bool IsEnabled
        {
            get { return _watcher.EnableRaisingEvents; }
        }

        public event EventHandler<FileChangedEventArgs> OnFileChanged;

        public FileListener(string path, string filter)
        {
            _watcher = new FileSystemWatcher(path, filter)
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.LastWrite
            };
        }

        public void Start()
        {
            if (IsEnabled) return;
            
            _watcher.Changed += OnChanged;
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;    
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            var eventHandler = OnFileChanged;
            if (eventHandler != null)
            {
                string directory = Path.GetDirectoryName(e.FullPath);
                var eventArgs = new FileChangedEventArgs { Directory = directory };
                eventHandler(sender, eventArgs);
            }
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
            }
        }
    }
}
