using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TucTuc
{
    public class TucTucMessage<T>
    {
        public Guid Id { get; set; }
        public DateTime SentAtUtc { get; set; }
        public T Payload { get; set; }
    }
}
