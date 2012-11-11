using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using ServiceStack.Text;

namespace TucTuc
{
    public interface ISerializer
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string data);
    }

    public class JsonSerializer : ISerializer
    {
        public virtual string Serialize<T>(T data)
        {
            return data.SerializeToString();
        }

        public virtual T Deserialize<T>(string data)
        {
            return (T)ServiceStack.Text.Json.JsonReader<T>.Parse(data);
        }
    }
}
