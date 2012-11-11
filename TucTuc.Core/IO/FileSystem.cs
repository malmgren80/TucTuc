using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TucTuc.IO
{
    public interface IFileSystem
    {
        void EnsureExists(string path);
        void WriteFile(string path, string data, Encoding encoding);
        string ReadFile(string path, Encoding encoding);
        IEnumerable<FileInfo> GetFiles(string path, string pattern);
    }

    public class FileSystem : IFileSystem
    {
        public void EnsureExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void WriteFile(string path, string data, Encoding encoding)
        {
            File.WriteAllText(path, data, encoding);
        }

        public string ReadFile(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public IEnumerable<FileInfo> GetFiles(string path, string pattern)
        {
            return new DirectoryInfo(path).GetFiles(pattern, SearchOption.TopDirectoryOnly);
        }
    }
}
