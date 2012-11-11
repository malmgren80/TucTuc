using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TucTuc
{
    [Serializable]
    public class TucTucMessage
    {
        public Guid Id { get; set; }
        public DateTime SentAtUtc { get; set; }
        public string Sender { get; set; }
        public object Payload { get; set; }
    }
}
