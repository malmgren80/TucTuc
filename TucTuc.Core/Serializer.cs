using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace TucTuc
{
    public interface ISerializer
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string data);
    }

    public class JsonSerializer : ISerializer
    {
        private readonly JavaScriptSerializer _serializer;

        public JsonSerializer()
        {
            _serializer = new JavaScriptSerializer();
        }

        public virtual string Serialize<T>(T data)
        {
            return _serializer.Serialize(data);
        }

        public virtual T Deserialize<T>(string data)
        {
            return _serializer.Deserialize<T>(data);
        }
    }
}
