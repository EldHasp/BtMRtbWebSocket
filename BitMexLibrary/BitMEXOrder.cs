using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary
{

    public class BitMEXOrder
    {
        public string orderID { get; set; }
        public string clOrdID { get; set; }
        public string clOrdLinkID { get; set; }
        public decimal? account { get; set; }
        public string symbol { get; set; }
        public string side { get; set; }
        public decimal? simpleOrderQty { get; set; }
        public decimal? orderQty { get; set; }
        public decimal? price { get; set; }
        public decimal? displayQty { get; set; }
        public decimal? stopPx { get; set; }
        public decimal? pegOffsetValue { get; set; }
        public string pegPriceType { get; set; }
        public string currency { get; set; }
        public string settlCurrency { get; set; }
        public string ordType { get; set; }
        public string timeInForce { get; set; }
        public string execInst { get; set; }
        public string contingencyType { get; set; }
        public string exDestination { get; set; }
        public string ordStatus { get; set; }
        public string triggered { get; set; }
        public bool? workingIndicator { get; set; }
        public string ordRejReason { get; set; }
        public decimal? simpleLeavesQty { get; set; }
        public decimal? leavesQty { get; set; }
        public decimal? simpleCumQty { get; set; }
        public decimal? cumQty { get; set; }
        public decimal? avgPx { get; set; }
        public string multiLegReportingype { get; set; }
        public string text { get; set; }
        public DateTime? transactTime { get; set; }
        public DateTime? timestamp { get; set; }

        public BitMEXOrder() { }

        public BitMEXOrder(string orderID, string clOrdID, string clOrdLinkID, decimal? account, string symbol, string side, decimal? simpleOrderQty, decimal? orderQty, decimal? price, decimal? displayQty, decimal? stopPx, decimal? pegOffsetValue, string pegPriceType, string currency, string settlCurrency, string ordType, string timeInForce, string execInst, string contingencyType, string exDestination, string ordStatus, string triggered, bool? workingIndicator, string ordRejReason, decimal? simpleLeavesQty, decimal? leavesQty, decimal? simpleCumQty, decimal? cumQty, decimal? avgPx, string multiLegReportingype, string text, DateTime? transactTime, DateTime? timestamp)
        {

            this.orderID = orderID;
            this.clOrdID = clOrdID;
            this.clOrdLinkID = clOrdLinkID;
            this.account = account;
            this.symbol = symbol;
            this.side = side;
            this.simpleOrderQty = simpleOrderQty;
            this.orderQty = orderQty;
            this.price = price;
            this.displayQty = displayQty;
            this.stopPx = stopPx;
            this.pegOffsetValue = pegOffsetValue;
            this.pegPriceType = pegPriceType;
            this.currency = currency;
            this.settlCurrency = settlCurrency;
            this.ordType = ordType;
            this.timeInForce = timeInForce;
            this.execInst = execInst;
            this.contingencyType = contingencyType;
            this.exDestination = exDestination;
            this.ordStatus = ordStatus;
            this.triggered = triggered;
            this.workingIndicator = workingIndicator;
            this.ordRejReason = ordRejReason;
            this.simpleLeavesQty = simpleLeavesQty;
            this.leavesQty = leavesQty;
            this.simpleCumQty = simpleCumQty;
            this.cumQty = cumQty;
            this.avgPx = avgPx;
            this.multiLegReportingype = multiLegReportingype;
            this.text = text;
            this.transactTime = transactTime;
            this.timestamp = timestamp;
        }
    }

}
