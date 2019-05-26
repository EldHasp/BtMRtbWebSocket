using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary.WebSocketJSON
{
    [DataContract]
    public class StatusClass
    {
        [DataMember(Name = "status", IsRequired = true)]
        public int Status { get; set; }
        [DataMember(Name = "error", IsRequired = false)]
        public string Error { get; set; }
        [DataMember(Name = "meta", IsRequired = false)]
        public object Meta { get; set; }
        [DataMember(Name = "request", IsRequired = false)]
        public OpClass Request { get; set; }
    }

}
