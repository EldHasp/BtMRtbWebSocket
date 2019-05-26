using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary.WebSocketJSON
{
    [DataContract]
    public class SuccessClass
    {
        [DataMember(Name = "success", IsRequired = true)]
        public bool Success { get; set; }
        [DataMember(Name = "request", IsRequired = true)]
        public OpClass Request { get; set; }
        [DataMember(Name = "subscribe", IsRequired = false)]
        public string Subscribe { get; set; }
    }

}
