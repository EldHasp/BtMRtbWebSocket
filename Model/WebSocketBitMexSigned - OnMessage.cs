using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace BitMexLibrary
{
    public partial class WebSocketBitMexSigned : OnPropertyChangedClass
    {

        public static WebSocketBitMexSigned WSocket { get; private set; }
        public static WebSocketBitMexSigned Create(string APIKey = "", string APISecret = "", bool RealNet = false)
        {
            if (WSocket != null)
            {
                if (WSocket.IsOpen)
                    WSocket.WS.Close();
                WSocket = null;
            }
            WSocket = new WebSocketBitMexSigned(APIKey, APISecret, RealNet);
            Thread.Sleep(1000);

            if (!WSocket.IsOpen)
                WSocket = null;
            return WSocket;
        }

        /// <summary>Возвращает открытый WebSocket</summary>
        public WebSocket WS { get; }

        /// <summary>Возвращает тип сети (Реальная - true).</summary>
        public bool RealNet { get; }



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

        List<TableJSON> listTableJSON = new List<TableJSON>();

        /// <summary>Обработчик события получения сообщения от сокета</summary>
        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            CountMessage++;
            long numMess = CountMessage;

            TimeLastMessage = DateTime.UtcNow;
            //try
            {
                if (e.Data.StartsWith("{\"table\":\"orderBook10\""))
                {
                    string symbol = "\"symbol\":\"" + WorkSymbol + "\"";
                    string bids = "bids\":[[";
                    string asks = "asks\":[[";
                    int indexSymb = (e.Data.IndexOf(symbol));
                    if (indexSymb >= 0)
                    {
                        int index = (e.Data.IndexOf(bids, indexSymb + symbol.Length));
                        if (index > 0)
                        {
                            int indEnd = e.Data.IndexOf(',', index);
                            string val = e.Data.Substring((index += bids.Length), indEnd - index);
                            MaxBuy = decimal.Parse(val, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                        }
                        index = (e.Data.IndexOf(asks, indexSymb + symbol.Length));
                        if (index > 0)
                        {
                            int indEnd = e.Data.IndexOf(',', index);
                            string val = e.Data.Substring((index += asks.Length), indEnd - index);
                            MinSell = decimal.Parse(val, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (e.Data.StartsWith("{\"table\":\"quote\""))
                {
                    string symbol = "\"symbol\":\"" + WorkSymbol + "\"";
                    string bidPrice = "bidPrice\":";
                    string askPrice = "askPrice\":";
                    int indexSymb = (e.Data.IndexOf(symbol));
                    if (indexSymb >= 0)
                    {
                        int index = (e.Data.IndexOf(bidPrice, indexSymb + symbol.Length));
                        if (index > 0)
                        {
                            int indEnd = e.Data.IndexOf(',', index);
                            string val = e.Data.Substring((index += bidPrice.Length), indEnd - index);
                            MaxBuy = decimal.Parse(val, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                        }
                        index = (e.Data.IndexOf(askPrice, indexSymb + symbol.Length));
                        if (index > 0)
                        {
                            int indEnd = e.Data.IndexOf(',', index);
                            string val = e.Data.Substring((index += askPrice.Length), indEnd - index);
                            MinSell = decimal.Parse(val, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                        }
                    }

                }
                else if (e.Data.StartsWith("{\"info\":"))
                {
                    if (JsonConvertExtension.TryDeserializeObject(e.Data, out InfoDocs infoDocs))
                    {
                        Console.WriteLine(infoDocs.Print());
                        InfoDocsList = InfoDocsList.Append(infoDocs);
                    }
                    else if (JsonConvertExtension.TryDeserializeObject(e.Data, out InfoHelp infoHelp))
                    {
                        Console.WriteLine(infoHelp.Print());
                    }
                }
                else if (e.Data.StartsWith("{\"table\":"))
                {

                    if (JsonConvertExtension.TryDeserializeObject(e.Data, out TableJSON table))
                    {
                        table.Number = numMess;
                        if (table.Data == null || table.Data.Length == 0)
                            ConvertTable(table);
                        else
                        {
                            lock (listTableJSON)
                            {
                                listTableJSON.Add(table);
                                if (table.Table == "order")
                                {
                                    //Console.WriteLine
                                    //         (
                                    //             $"№{table.Number} ({DateTime.Now.Ticks}) ->" +
                                    //             $"table={{{table.Table}, {table.Action}, {table.Data[0]["timestamp"]:HH:mm:ss.ffffff}}},"
                                    //         );
                                }
                            }
                            while (listTableJSON.Count > 0)
                            {
                                Thread.Sleep(100);
                                lock (listTableJSON)
                                {
                                    if (listTableJSON.Count > 0)
                                    {
                                        TableJSON tableFirst = listTableJSON
                                            .OrderBy(t => t.Data[0]["timestamp"])
                                            .ThenBy(t => t.Action == "partial" ? 0 : t.Action == "insert" ? 1 : t.Action == "update" ? 2 : 3)
                                            .ThenBy(t => !t.Data[0].ContainsKey("workingIndicator") ? 0 : (bool)t.Data[0]["workingIndicator"] ? 1 : 2)
                                            .First();
                                        if (tableFirst.Table == "order")
                                        {
                                            Console.WriteLine
                                                (
                                                    $"tableFirst={{{tableFirst.Table}, {tableFirst.Action}, {tableFirst.Data[0]["orderID"]}, {tableFirst.Data[0]["timestamp"]:HH:mm:ss.ffffff}}}"
                                                );
                                        }                                        //if (table.Table == "order" || tableFirst.Table == "order")
                                        //{
                                        //    Console.WriteLine
                                        //        (
                                        //            $"№{table.Number} ({DateTime.Now.Ticks}) ->" +
                                        //            $"table={{{table.Table}, {table.Action}, {table.Data[0]["timestamp"]:HH:mm:ss.ffffff}}}," +
                                        //            $"tableFirst={{{tableFirst.Table}, {tableFirst.Action}, {tableFirst.Data[0]["timestamp"]:HH:mm:ss.ffffff}}}"
                                        //        );
                                        //}
                                        ConvertTable(tableFirst);
                                        listTableJSON.Remove(tableFirst);
                                    }
                                }
                            }

                        }

                        //Console.WriteLine($"TableMargin: Баланс ( * 100 млн.): {tableMargin.data["walletBalance"]}");
                    }
                    else
                    {

                    }

                }
                //else if (JsonConvertExtension.TryDeserializeObject(e.Data, out TableOrderBookArr tableOrderBook))
                //{
                //    //Console.WriteLine($"tableOrderBook: {tableOrderBook.Table}, {tableOrderBook.Action}, {tableOrderBook.Data[0].Asks[0][0]}, {tableOrderBook.Data[0].Bids[0][0]}");
                //    if (tableOrderBook.Table == "orderBook10" && tableOrderBook.Data.FirstOrDefault(data => data.Symbol == WorkSymbol) is OrderBookArr orderBook)
                //    {
                //        OrderBookList orderBookList = OrderBookList.FromArr(orderBook);
                //        OrderBook10Asks = orderBookList.Asks;
                //        OrderBook10Bids = orderBookList.Bids;
                //    }
                //}
                else if (e.Data.StartsWith("{\"status\":"))
                {
                    if (JsonConvertExtension.TryDeserializeObject(e.Data, out StatusClass status)  /*&& status.Status!=null*/)
                    {
                        if (status.Status == 401)
                        {
                            switch (status.Error)
                            {
                                case "Invalid API Key.":
                                    InvalidAPIKey = true;
                                    Authorization = false;
                                    break;
                                case "Signature not valid.":
                                    SignatureNotValid = true;
                                    Authorization = false;
                                    break;
                            }
                        }
                        Console.WriteLine($"StatusClass: {status.Status}, {status.Error}, {status.Request?.Op}, {status.Request?.Args[0]}");
                    }
                }
                else if (e.Data.StartsWith("{\"success\":"))
                {
                    if (JsonConvertExtension.TryDeserializeObject(e.Data, out SuccessClass success)  /*&& status.Status!=null*/)
                    {
                        if (success.Request.Op == "authKeyExpires")
                            Authorization = success.Success;
                        Console.WriteLine($"SuccessClass: {success.Success}, {success.Subscribe}, {success.Request.Op}, {success.Request.Args[0]}");
                    }
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
                //}
            }
            //catch (Exception)
            //{
            //    MessageBox.Show("Какой-то сбой при  разборе ответа от сервера");
            //}

        }

        private void ConvertTable(TableJSON tableFirst)
        {
            if (DataWallet.TryFromTable(tableFirst, Wallet, out DataWallet outW))
            {
                Console.WriteLine($"TableWallet Amount: {outW.Amount}");
                Wallet = outW;
            }
            else if (DataMargin.TryFromTable(tableFirst, Margin, out DataMargin outM))
            {
                Console.WriteLine($"TableMargin Amount: {outM.Amount}");
                Margin = outM;
            }
            else if (Position.TryFromTable(tableFirst, Positions, out ObservableCollection<Position> outP))
            {
                //Console.WriteLine($"TablePositions Positions.Data.Length: {table.Data.Length}");
                //Console.WriteLine("************************************\r\n" + e.Data + "\r\n************************************");
                Positions = outP;
                OnPropertyChanged("ChangedPositions");
            }
            else if (TableOrder.TryFromTable(tableFirst, Orders, out ObservableCollection<TableOrder> outO))
            {
                //Console.WriteLine($"TableOrders Orders.Data.Length: {table.Data.Length}");
                Orders = outO;
            }
            else
            {

            }

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
