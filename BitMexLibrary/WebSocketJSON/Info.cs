using CommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary.WebSocketJSON
{
    [DataContract]
    public class InfoDocs
    {
        [DataMember(Name = "info", IsRequired = true)]
        public string Info { get; set; }
        [DataMember(Name = "version", IsRequired = false)]
        public DateTime Version { get; set; }
        [DataMember(Name = "timestamp", IsRequired = false)]
        public DateTime TimeStamp { get; set; }
        [DataMember(Name = "docs", IsRequired = true)]
        public string Docs { get; set; }
        [DataMember(Name = "limit", IsRequired = false)]
        public Remainings Limit { get; set; }

        public string Print()
            => $"Info=\"{Info}\", Version=\"{Version}\", TimeStamp=\"{TimeStamp}\", Docs=\"{Docs}\", Limit.Remaining=\"{Limit.Remaining}\"";
    }

    [DataContract]
    public class Remainings
    {
        [DataMember(Name = "remaining", IsRequired = false)]
        public int Remaining { get; set; }
    }


    [DataContract]
    public class InfoHelp
    {
        [DataMember(Name = "info", IsRequired = true)]
        public string Info { get; set; }
        [DataMember(Name = "usage", IsRequired = true)]
        public string Usage { get; set; }
        [DataMember(Name = "ops", IsRequired = false)]
        public string[] Ops { get; set; }
        [DataMember(Name = "subscribe", IsRequired = true)]
        public string Subscribe { get; set; }
        [DataMember(Name = "subscriptionSubjects", IsRequired = false)]
        public SubscriptionSubjectsClass SubscriptionSubjects { get; set; }

        public string Print()
            => $"Info=\"{Info}\", Usage=\"{Usage}\", Ops=\"[{string.Join(", ",Ops)}]\", Subscribe=\"{Subscribe}\", SubscriptionSubjects=\"{{{SubscriptionSubjects.Print()}}}\"";
    }

    [DataContract]
    public class SubscriptionSubjectsClass
    {
        [DataMember(Name = "authenticationRequired", IsRequired = true)]
        public string[] AuthenticationRequired { get; set; }
        [DataMember(Name = "public", IsRequired = true)]
        public string[] Public { get; set; }

        public string Print() => $"AuthenticationRequired=\"[{string.Join(", ",AuthenticationRequired)}]\", Public=\"[{string.Join(", ",Public)}]\"";
    }

}
