using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace TucTuc
{
    public interface ITransport
    {
        void Message(string data, string endpoint);
    }

    public class FileTransport : ITransport
    {
        public void Message(string data, string endpoint)
        {
            string directoryPath = Path.GetFullPath(endpoint);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fileName = string.Format("{0}.msg", Guid.NewGuid());
            string fullPath = Path.Combine(directoryPath, fileName);
            
            File.WriteAllText(fullPath, data, Encoding.Unicode);
        }
    }
}
