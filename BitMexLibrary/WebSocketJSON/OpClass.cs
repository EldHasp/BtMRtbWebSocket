using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary.WebSocketJSON
{
    [DataContract]
    public class OpClass
    {
        [DataMember(Name = "op", IsRequired = true)]
        public string Op { get; set; }
        [DataMember(Name = "args", IsRequired = true)]
        public object[] Args { get; set; }
    }
    [DataContract]
    public class OpClassDict
    {
        [DataMember(Name = "op", IsRequired = true)]
        public string Op { get; set; }
        [DataMember(Name = "args", IsRequired = true)]
        public Dictionary<string,object> Args { get; set; }
    }
}
