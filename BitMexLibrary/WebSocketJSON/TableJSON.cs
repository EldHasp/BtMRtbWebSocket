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
    public class TableJSON : OnPropertyChangedClass
    {
        [DataMember(Name = "table", IsRequired = true)] public string Table { get; set; }
        [DataMember(Name = "action", IsRequired = true)] public string Action { get; set; }
        [DataMember(Name = "keys", IsRequired = false)] public string[] Keys { get; set; }
        [DataMember(Name = "types", IsRequired = false)] public Dictionary<string, string> Types { get; set; }
        [DataMember(Name = "foreignKeys", IsRequired = false)] public Dictionary<string, string> ForeignKeys { get; set; }
        [DataMember(Name = "attributes", IsRequired = false)] public Dictionary<string, string> Attributes { get; set; }
        [DataMember(Name = "filter", IsRequired = false)] public Dictionary<string, string> Filter { get; set; }
        [DataMember(Name = "data", IsRequired = true)] public Dictionary<string, object>[] Data { get; set; }
        public long Number { get; set; }
    }

    //[DataContract]
    //public class Foreignkeys
    //{
    //    public string symbol { get; set; }
    //    public string side { get; set; }
    //    public string ordStatus { get; set; }
    //}

    //[DataContract]
    //public class Attributes
    //{
    //    [DataMember(Name = "account", IsRequired = true)] public string Account { get; set; }
    //    [DataMember(Name = "currency", IsRequired = true)] public string Currency { get; set; }
    //}

    //[DataContract]
    //public class Filter
    //{
    //    [DataMember(Name = "account", IsRequired = true)] public int Account { get; set; }
    //}


    //public class Rootobject
    //{
    //    public string table { get; set; }
    //    public string action { get; set; }
    //    public string[] keys { get; set; }
    //    public Types types { get; set; }
    //    public Foreignkeys foreignKeys { get; set; }
    //    public Attributes attributes { get; set; }
    //    public Filter filter { get; set; }
    //    public Datum[] data { get; set; }
    //}

    //public class Types
    //{
    //    public string orderID { get; set; }
    //    public string clOrdID { get; set; }
    //    public string clOrdLinkID { get; set; }
    //    public string account { get; set; }
    //    public string symbol { get; set; }
    //    public string side { get; set; }
    //    public string simpleOrderQty { get; set; }
    //    public string orderQty { get; set; }
    //    public string price { get; set; }
    //    public string displayQty { get; set; }
    //    public string stopPx { get; set; }
    //    public string pegOffsetValue { get; set; }
    //    public string pegPriceType { get; set; }
    //    public string currency { get; set; }
    //    public string settlCurrency { get; set; }
    //    public string ordType { get; set; }
    //    public string timeInForce { get; set; }
    //    public string execInst { get; set; }
    //    public string contingencyType { get; set; }
    //    public string exDestination { get; set; }
    //    public string ordStatus { get; set; }
    //    public string triggered { get; set; }
    //    public string workingIndicator { get; set; }
    //    public string ordRejReason { get; set; }
    //    public string simpleLeavesQty { get; set; }
    //    public string leavesQty { get; set; }
    //    public string simpleCumQty { get; set; }
    //    public string cumQty { get; set; }
    //    public string avgPx { get; set; }
    //    public string multiLegReportingType { get; set; }
    //    public string text { get; set; }
    //    public string transactTime { get; set; }
    //    public string timestamp { get; set; }
    //}

    //public class Attributes
    //{
    //    public string orderID { get; set; }
    //    public string account { get; set; }
    //    public string ordStatus { get; set; }
    //    public string workingIndicator { get; set; }
    //}

    //public class Filter
    //{
    //    public int account { get; set; }
    //}

    //public class Datum
    //{
    //    public string orderID { get; set; }
    //    public string clOrdID { get; set; }
    //    public string clOrdLinkID { get; set; }
    //    public int account { get; set; }
    //    public string symbol { get; set; }
    //    public string side { get; set; }
    //    public object simpleOrderQty { get; set; }
    //    public int orderQty { get; set; }
    //    public float price { get; set; }
    //    public object displayQty { get; set; }
    //    public object stopPx { get; set; }
    //    public object pegOffsetValue { get; set; }
    //    public string pegPriceType { get; set; }
    //    public string currency { get; set; }
    //    public string settlCurrency { get; set; }
    //    public string ordType { get; set; }
    //    public string timeInForce { get; set; }
    //    public string execInst { get; set; }
    //    public string contingencyType { get; set; }
    //    public string exDestination { get; set; }
    //    public string ordStatus { get; set; }
    //    public string triggered { get; set; }
    //    public bool workingIndicator { get; set; }
    //    public string ordRejReason { get; set; }
    //    public object simpleLeavesQty { get; set; }
    //    public int leavesQty { get; set; }
    //    public object simpleCumQty { get; set; }
    //    public int cumQty { get; set; }
    //    public object avgPx { get; set; }
    //    public string multiLegReportingType { get; set; }
    //    public string text { get; set; }
    //    public DateTime transactTime { get; set; }
    //    public DateTime timestamp { get; set; }
    //}

}
