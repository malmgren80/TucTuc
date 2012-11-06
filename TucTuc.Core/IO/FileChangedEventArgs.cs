using System;

namespace TucTuc.IO
{
    public class FileChangedEventArgs : EventArgs
    {
        public string Directory { get; set; }
    }
}