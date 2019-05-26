using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace BitMexLibrary
{
    public partial class WebSocketBitMexUnSigned : OnPropertyChangedClass
    {


        const string UrlBitMexReal = "wss://www.bitmex.com/realtime";
        const string UrlBitMexTest = "wss://testnet.bitmex.com/realtime";

        /// <summary>Возвращает открытый WebSocket</summary>
        public WebSocket WS { get; }

        /// <summary>Возвращает тип сети (Реальная - true).</summary>
        public bool RealNet { get; }



        public WebSocketBitMexUnSigned(bool RealNet)
        {
            //string tmp = "{\"table\":\"margin\",\"action\":\"partial\",\"keys\":[\"account\",\"currency\"],\"types\":{\"account\":\"long\",\"currency\":\"symbol\",\"riskLimit\":\"long\",\"prevState\":\"symbol\",\"state\":\"symbol\",\"action\":\"symbol\",\"amount\":\"long\",\"pendingCredit\":\"long\",\"pendingDebit\":\"long\",\"confirmedDebit\":\"long\",\"prevRealisedPnl\":\"long\",\"prevUnrealisedPnl\":\"long\",\"grossComm\":\"long\",\"grossOpenCost\":\"long\",\"grossOpenPremium\":\"long\",\"grossExecCost\":\"long\",\"grossMarkValue\":\"long\",\"riskValue\":\"long\",\"taxableMargin\":\"long\",\"initMargin\":\"long\",\"maintMargin\":\"long\",\"sessionMargin\":\"long\",\"targetExcessMargin\":\"long\",\"varMargin\":\"long\",\"realisedPnl\":\"long\",\"unrealisedPnl\":\"long\",\"indicativeTax\":\"long\",\"unrealisedProfit\":\"long\",\"syntheticMargin\":\"long\",\"walletBalance\":\"long\",\"marginBalance\":\"long\",\"marginBalancePcnt\":\"float\",\"marginLeverage\":\"float\",\"marginUsedPcnt\":\"float\",\"excessMargin\":\"long\",\"excessMarginPcnt\":\"float\",\"availableMargin\":\"long\",\"withdrawableMargin\":\"long\",\"timestamp\":\"timestamp\",\"grossLastValue\":\"long\",\"commission\":\"float\"},\"foreignKeys\":{},\"attributes\":{\"account\":\"sorted\",\"currency\":\"grouped\"},\"filter\":{\"account\":164829},\"data\":[{\"account\":164829,\"currency\":\"XBt\",\"riskLimit\":1000000000000,\"prevState\":\"\",\"state\":\"\",\"action\":\"\",\"amount\":940373,\"pendingCredit\":0,\"pendingDebit\":0,\"confirmedDebit\":0,\"prevRealisedPnl\":-39,\"prevUnrealisedPnl\":0,\"grossComm\":0,\"grossOpenCost\":0,\"grossOpenPremium\":0,\"grossExecCost\":0,\"grossMarkValue\":0,\"riskValue\":0,\"taxableMargin\":0,\"initMargin\":0,\"maintMargin\":0,\"sessionMargin\":0,\"targetExcessMargin\":0,\"varMargin\":0,\"realisedPnl\":0,\"unrealisedPnl\":0,\"indicativeTax\":0,\"unrealisedProfit\":0,\"syntheticMargin\":null,\"walletBalance\":940373,\"marginBalance\":940373,\"marginBalancePcnt\":1,\"marginLeverage\":0,\"marginUsedPcnt\":0,\"excessMargin\":940373,\"excessMarginPcnt\":1,\"availableMargin\":940373,\"withdrawableMargin\":940373,\"timestamp\":\"2019 - 01 - 21T21: 53:11.417Z\",\"grossLastValue\":0,\"commission\":null}]}";

            //var ser = JsonConvertExtension.TryDeserializeObject(tmp, out TableWallet table);

            this.RealNet = RealNet;
            WS = new WebSocket
                (
                    (RealNet ? UrlBitMexReal : UrlBitMexTest)
                    + "?subscribe=orderBook10:" + WorkSymbol
                );
            WS.OnClose += Ws_OnClose;
            WS.OnError += Ws_OnError;
            WS.OnMessage += Ws_OnMessageAsync;
            WS.OnOpen += Ws_OnOpen;

            WS.Connect();

        }

        /// <summary>Обработчик события открытия сокета</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ws_OnOpen(object sender, EventArgs e)
        {
            IsOpen = true;
            IsClose = false;
        }

        /// <summary>Обработчик события получения сообщения от сокета</summary>
        private async void Ws_OnMessageAsync(object sender, MessageEventArgs e)
        {
            await Task.Run(() => Ws_OnMessage(sender, e));
        }

        /// <summary>Обработчик события получения сообщения от сокета</summary>
        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            CountMessage++;
            DateTime timeMessage = DateTime.UtcNow;
            if (e.Data.StartsWith("{\"table\":\"orderBook10\""))
            {
                string bids = "bids\":[[";
                string asks = "asks\":[[";
                int index = (e.Data.IndexOf(bids));
                if (index > 0)
                {
                    int indEnd = e.Data.IndexOf(',', index);
                    string val = e.Data.Substring((index += bids.Length), indEnd - index);
                    MaxBuy = decimal.Parse(val, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                }
                index = (e.Data.IndexOf(asks));
                if (index > 0)
                {
                    int indEnd = e.Data.IndexOf(',', index);
                    string val = e.Data.Substring((index += asks.Length), indEnd - index);
                    MinSell = decimal.Parse(val, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                }
            }

            //if (e.Data.StartsWith("{\"table\":\"orderBook10\"") && e.Data.Contains("partial"))
            //{

            //}

            //if ((timeMessage - TimeLastMessage).TotalMilliseconds < 500)
            //    return;

            TimeLastMessage = timeMessage;

            if (JsonConvertExtension.TryDeserializeObject(e.Data, out InfoDocs infoDocs) /*&& infoDocs.Info !=null && infoDocs.Docs !=null*/)
            {
                Console.WriteLine(infoDocs.Print());
            }
            else if (JsonConvertExtension.TryDeserializeObject(e.Data, out TableOrderBookArr tableOrderBook))
            {
                //Console.WriteLine($"tableOrderBook: {tableOrderBook.Table}, {tableOrderBook.Action}, {tableOrderBook.Data[0].Asks[0][0]}, {tableOrderBook.Data[0].Bids[0][0]}");
                if (tableOrderBook.Table == "orderBook10" && tableOrderBook.Data.FirstOrDefault(data => data.Symbol == WorkSymbol) is OrderBookArr orderBook)
                {
                    OrderBookList orderBookList = OrderBookList.FromArr(orderBook);
                    switch (tableOrderBook.Action)
                    {
                        case "partial":
                            {
                                OrderBook10Asks = orderBookList.Asks;
                                OrderBook10Bids = orderBookList.Bids;
                                break;
                            }
                        case "update":
                            {
                                if (OrderBook10Asks != default)
                                {
                                    foreach ((OrderBook s, OrderBook f) tupl in OrderBook10Asks.Zip(orderBookList.Asks, (s, f) => (s, f)))
                                        tupl.s.CopyFrom(tupl.f);
                                    foreach ((OrderBook s, OrderBook f) tupl in OrderBook10Bids.Zip(orderBookList.Bids, (s, f) => (s, f)))
                                        tupl.s.CopyFrom(tupl.f);
                                }
                                break;
                            }
                    }
                }
            }
            else if (JsonConvertExtension.TryDeserializeObject(e.Data, out StatusClass status)  /*&& status.Status!=null*/)
            {
                Console.WriteLine($"StatusClass: {status.Status}, {status.Error}, {status.Request.Op}, {status.Request.Args[0]}");
            }
            else if (JsonConvertExtension.TryDeserializeObject(e.Data, out SuccessClass success)  /*&& status.Status!=null*/)
            {
                Console.WriteLine($"SuccessClass: {success.Success}, {success.Subscribe}, {success.Request.Op}, {success.Request.Args[0]}");
            }
            else
                Console.WriteLine("e.Data = " + e.Data);


            //messCount++;
            //WebScocketLastMessage = DateTime.UtcNow;
            //try
            //{
            //    JObject Message = JObject.Parse(e.Data);
            //    if (Message.ContainsKey("table"))
            //    {
            //        if ((string)Message["table"] == "trade")
            //        {
            //            //if (Message.ContainsKey("data"))
            //            //{
            //            //    JArray TD = (JArray)Message["data"];
            //            //    if (TD.Any())
            //            //    {
            //            //        decimal Price = (decimal)TD.Children().LastOrDefault()["price"];
            //            //        string Symbol = (string)TD.Children().LastOrDefault()["symbol"];
            //            //        Prices[Symbol] = Price;
            //            //    }
            //            //}
            //        }
            //        else if ((string)Message["table"] == "orderBook10")
            //        {
            //            if (Message.ContainsKey("data"))
            //            {

            //                var arrOrderBook = JsonConvert.DeserializeObject<TableOrderBookArr>(e.Data);
            //                var listOrderBook = TableOrderBookList.FormArr(arrOrderBook);

            //                //    JArray TD = (JArray)Message["data"];
            //                //    if (TD.Any())
            //                //    {
            //                //        JArray TDBids = (JArray)TD[0]["bids"];
            //                //        if (TDBids.Any())
            //                //        {
            //                //            List<OrderBook> OB = new List<OrderBook>();
            //                //            foreach (JArray i in TDBids)
            //                //            {
            //                //                OrderBook OBI = new OrderBook
            //                //                {
            //                //                    Price = (decimal)i[0],
            //                //                    Size = (int)i[1]
            //                //                };
            //                //                OB.Add(OBI);
            //                //            }

            //                //            OrderBookTopBids = OB;
            //                //        }

            //                //        JArray TDAsks = (JArray)TD[0]["asks"];
            //                //        if (TDAsks.Any())
            //                //        {
            //                //            List<OrderBook> OB = new List<OrderBook>();
            //                //            foreach (JArray i in TDAsks)
            //                //            {
            //                //                OrderBook OBI = new OrderBook
            //                //                {
            //                //                    Price = (decimal)i[0],
            //                //                    Size = (int)i[1]
            //                //                };
            //                //                OB.Add(OBI);
            //                //            }

            //                //            OrderBookTopAsks = OB;
            //                //        }
            //                //    }
            //            }
            //        }
            //        else if ((string)Message["table"] == "position")
            //        {
            //            //// PARSE
            //            //if (Message.ContainsKey("data"))
            //            //{
            //            //    JArray TD = (JArray)Message["data"];
            //            //    if (TD.Any())
            //            //    {
            //            //        if (TD.Children().LastOrDefault()["symbol"] != null)
            //            //        {
            //            //            SymbolPosition.Symbol = (string)TD.Children().LastOrDefault()["symbol"];
            //            //        }
            //            //        if (TD.Children().LastOrDefault()["currentQty"] != null)
            //            //        {
            //            //            SymbolPosition.CurrentQty = (int?)TD.Children().LastOrDefault()["currentQty"];

            //            //        }
            //            //        if (TD.Children().LastOrDefault()["avgEntryPrice"] != null)
            //            //        {
            //            //            SymbolPosition.AvgEntryPrice = (decimal?)TD.Children().LastOrDefault()["avgEntryPrice"];

            //            //        }
            //            //        if (TD.Children().LastOrDefault()["markPrice"] != null)
            //            //        {
            //            //            SymbolPosition.MarkPrice = (decimal?)TD.Children().LastOrDefault()["markPrice"];

            //            //        }
            //            //        if (TD.Children().LastOrDefault()["liquidationPrice"] != null)
            //            //        {
            //            //            SymbolPosition.LiquidationPrice = (decimal?)TD.Children().LastOrDefault()["liquidationPrice"];
            //            //        }
            //            //        if (TD.Children().LastOrDefault()["leverage"] != null)
            //            //        {
            //            //            SymbolPosition.Leverage = (decimal?)TD.Children().LastOrDefault()["leverage"];

            //            //        }
            //            //        if (TD.Children().LastOrDefault()["unrealisedPnl"] != null)
            //            //        {
            //            //            SymbolPosition.UnrealisedPnl = (decimal?)TD.Children().LastOrDefault()["unrealisedPnl"];
            //            //        }
            //            //        if (TD.Children().LastOrDefault()["unrealisedPnlPcnt"] != null)
            //            //        {
            //            //            SymbolPosition.UnrealisedPnlPcnt = (decimal?)TD.Children().LastOrDefault()["unrealisedPnlPcnt"];

            //            //        }

            //            //    }
            //            //}
            //        }
            //        else if ((string)Message["table"] == "margin")
            //        {
            //            //    if (Message.ContainsKey("data"))
            //            //    {
            //            //        JArray TD = (JArray)Message["data"];
            //            //        if (TD.Any())
            //            //        {
            //            //            try
            //            //            {
            //            //                Balance = ((decimal)TD.Children().LastOrDefault()["walletBalance"] / 100000000);
            //            //                UpdateBalanceAndTime();
            //            //            }
            //            //            catch (Exception /*ex*/)
            //            //            {

            //            //            }
            //            //        }
            //            //    }
            //        }
            //    }
            //    else if (Message.ContainsKey("info") && Message.ContainsKey("docs"))
            //    {
            //        //string WebSocketInfo = "Websocket Info: " + Message["info"].ToString() + " " + Message["docs"].ToString();
            //        //UpdateWebSocketInfo(WebSocketInfo);

            //        InfoDocs info = JsonConvert.DeserializeObject<InfoDocs>(e.Data);
            //        Console.WriteLine(info.Print());
            //    }
            //}
            //catch (Exception /*ex*/)
            //{
            //    //MessageBox.Show(ex.Message);
            //}
        }

        /// <summary>Обработчик события получения ошибки от сокета</summary>
        private void Ws_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"WebSocket ERROR: \"{e.Message}\"");
        }

        /// <summary>Обработчик события закрытия сокета</summary>
        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            IsOpen = false;
            IsClose = true;
        }

    }
}
