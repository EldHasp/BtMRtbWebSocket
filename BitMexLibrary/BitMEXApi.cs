using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Linq;
using Newtonsoft.Json;
using CommLibrary;
using System.Xml.Linq;
using CommLibrary.Enums;
using BitMexLibrary.Enums;

namespace BitMexLibrary
{

    public class BitMEXApi : OnPropertyChangedClass
    {
        public struct QueryStruct
        {
            public int Number;
            public DateTime Time;
            public string Query;
        }

        private DateTime timeLastQuery;

        public DateTime TimeLastQuery
        {
            get => timeLastQuery;
            set
            {
                timeLastQuery = value;
                DeltaTime = DateTime.UtcNow - value;
            }
        }

        public static DateTime RealTime => DateTime.UtcNow - DeltaTime;

        public static TimeSpan DeltaTime { get; private set; }


        public int CountQuery { get => _countQuery; private set { _countQuery = value; OnPropertyChanged(); } }

        public int NumberQuery { get; private set; } = 1;
        public IEnumerable<QueryStruct> Queries { get; private set; } = Enumerable.Empty<QueryStruct>();

        private void AddQuery(string Query)
        {
            DateTime nowTime = RealTime;
            Queries = Queries.Append
                   (
                       new QueryStruct()
                       {
                           Number = ++NumberQuery,
                           Time = nowTime,
                           Query = Query
                       }
                   );
            CountQuery = Queries.Count(quer => (nowTime - quer.Time).TotalSeconds < 301);
        }

        private readonly string domain = "https://www.bitmex.com";
        private const string domainTest = "https://testnet.bitmex.com";
        private const string domainReal = "https://www.bitmex.com";
        private readonly string apiKey;
        private readonly string apiSecret;
        private readonly int rateLimit;

        #region Setting
        readonly bool Settings_OverloadRetry = false;
        readonly int Settings_OverloadRetryAttempts = 0;
        //readonly List<string> errors = new List<string>();

        #endregion

        public BitMEXApi(string bitmexKey = "", string bitmexSecret = "", bool RealWork = false, int rateLimit = 5000)
        {
            this.apiKey = bitmexKey;
            this.apiSecret = bitmexSecret;
            this.rateLimit = rateLimit;
            domain = RealWork ? domainReal : domainTest;
        }

        private int RetryAttempts(string res)
        {
            int att = 0;

            if (res.Contains("Unable to cancel order due to existing state"))
            {
                att = 0;
            }
            else if (res.Contains("The system is currently overloaded. Please try again later."))
            {
                if (Settings_OverloadRetry)
                {
                    att = Settings_OverloadRetryAttempts;
                }
                else
                {
                    att = 0;
                }
            }
            else if (res.Contains("error"))
            {
                att = 0;
            }

            return att;
        }

        public decimal GetAccountBalance()
        {
            var param = new Dictionary<string, string>
            {
                ["currency"] = "XBt"
            };
            string res = Query("GET", "/user/margin", param, true);
            if (res.Contains("error"))
            {
                return -1;
            }
            else
            {
                try
                {
                    return Convert.ToDecimal(JsonConvert.DeserializeObject<Margin>(res).UsefulBalance); // useful balance is the balance with full decimal places.

                }
                catch (Exception)
                {
                    return 0;
                }
                // default wallet balance doesn't show the decimal places like it should.
            }

        }

        public Candles GetCandleHistory(string symbol, int count, BinSizeEnum binSize, bool partial = false, int? start = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            var param = new Dictionary<string, string>
            {
                ["symbol"] = symbol,
                ["count"] = count.ToString(),
                ["reverse"] = true.ToString().ToLower(),
                ["partial"] = partial.ToString().ToLower(),
                ["binSize"] = binSize.Print()
            };
            if (start != null)
                param["start"] = start.ToString();

            if (startTime != null)
                param["startTime"] = startTime.Value.ToString("O");

            if (endTime != null)
                param["endTime"] = endTime.Value.ToString("O");

            string res = Query("GET", "/trade/bucketed", param);
            if (res.Substring(0, 10).Contains("error"))
            {
                Error.LastError = JsonConvert.DeserializeObject<RootError>(res).Error;
                return null;
            }
            Candles ret = JsonConvert.DeserializeObject<Candles>(res);
            ret.Sort((x, y) => (x.TimeStamp).CompareTo(y.TimeStamp));
            ret.BinSize = binSize;
            return ret;
        }

        public Candle GetCandleLast(string symbol, BinSizeEnum binSize, bool partial = false, DateTime? endTime = null)
        {
            var param = new Dictionary<string, string>
            {
                ["symbol"] = symbol,
                ["count"] = "1",
                ["reverse"] = true.ToString().ToLower(),
                ["partial"] = partial.ToString().ToLower(),
                ["binSize"] = binSize.Print()
            };
            if (endTime != null)
                param["endTime"] = endTime.Value.ToString("O");
            string res = Query("GET", "/trade/bucketed", param);
            if (res.Substring(0, 10).Contains("error"))
            {
                Error.LastError = JsonConvert.DeserializeObject<RootError>(res).Error;
                return null;
            }
            else if (res.TryParseXML(out XElement xRes))
            {
                Error.LastError = new Error(xRes);
                return null;
            }
            CandlesTA ret = JsonConvert.DeserializeObject<CandlesTA>(res);
            return ret[0];
        }


        //public List<Candle> GetCandleHistory(string symbol, string size, DateTime Start = new DateTime())
        //{
        //    var param = new Dictionary<string, string>
        //    {
        //        ["symbol"] = symbol,
        //        ["count"] = 500.ToString(), // 500 max
        //        ["reverse"] = "false"
        //    };
        //    if (Start != new DateTime())
        //    {
        //        param["startTime"] = Start.ToString();
        //    }
        //    param["binSize"] = size;
        //    string res = Query("GET", "/trade/bucketed", param);
        //    int RetryAttemptCount = 0;
        //    int MaxRetries = RetryAttempts(res);
        //    while (res.Contains("error") && RetryAttemptCount < MaxRetries)
        //    {
        //        errors.Add(res);
        //        Thread.Sleep(500); // Force app to wait 500ms
        //        res = Query("GET", "/trade/bucketed", param);
        //        RetryAttemptCount++;
        //        if (RetryAttemptCount == MaxRetries)
        //        {
        //            errors.Add("Max rety attempts of " + MaxRetries.ToString() + " reached.");
        //            break;
        //        }
        //    }
        //    try
        //    {
        //        return JsonConvert.DeserializeObject<List<Candle>>(res).OrderBy(a => a.TimeStamp).ToList();
        //    }
        //    catch (Exception /*ex*/)
        //    {
        //        return new List<Candle>();
        //    }

        //}


        /// <summary>Преобразование словаря парметров в формат QueryData</summary>
        /// <param name="param">Словарь параметров</param>
        /// <returns>Преобразованное значение</returns>
        private string BuildQueryData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";
            return string.Join("&", param.Select(item => $"{item.Key}={item.Value}"));

            //var entries = new List<string>();
            //foreach (var item in param)
            //    entries.Add(string.Format($"{item.Key}={item.Value}"));

            //return string.Join("&", entries);


            //StringBuilder b = new StringBuilder();
            //foreach (var item in param)
            //    b.Append(string.Format("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value)));

            //try { return b.ToString().Substring(1); }
            //catch (Exception) { return ""; }
        }

        /// <summary>Преобразование словаря парметров в формат JSON</summary>
        /// <param name="param">Словарь параметров</param>
        /// <returns>Преобразованное значение</returns>
        private string BuildJSON(Dictionary<string, string> param)
        {
            if (param == null)
                return "";
            return $"{{{string.Join(",", param.Select(item => $"\"{item.Key}\":\"{item.Value}\""))}}}";

            //var entries = new List<string>();
            //foreach (var item in param)
            //    entries.Add(string.Format($"\"{item.Key}\":\"{item.Value}\""));

            //return "{" + string.Join(",", entries) + "}";
        }

        /// <summary>Перевод последовательности байтов в шестнадцатеричный код</summary>
        /// <param name="ba">Последовательность байтов</param>
        /// <returns></returns>
        public static string ByteArrayToString(IEnumerable<byte> ba)
        {
            return string.Join("", ba.Select(b => b.ToString("x2")));
            //StringBuilder hex = new StringBuilder(ba.Count() * 2);
            //foreach (byte b in ba)
            //    hex.AppendFormat("{0:x2}", b);
            //return hex.ToString();
        }

        private long GetExpires()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600; // set expires one hour in the future
        }

        private string Query(string method, string function, Dictionary<string, string> param = null, bool auth = false, bool json = false)
        {
            string paramData = json ? BuildJSON(param) : BuildQueryData(param);
            string url = "/api/v1" + function + ((method == "GET" && paramData != "") ? "?" + paramData : "");
            string postData = (method != "GET") ? paramData : "";

            string queryUrl = domain + url;
            AddQuery(queryUrl);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(queryUrl);
            webRequest.Method = method;

            if (auth)
            {
                string expires = GetExpires().ToString();
                string message = method + url + expires + postData;
                byte[] signatureBytes = Hmacsha256(Encoding.UTF8.GetBytes(apiSecret), Encoding.UTF8.GetBytes(message));
                string signatureString = ByteArrayToString(signatureBytes);

                webRequest.Headers.Add("api-expires", expires);
                webRequest.Headers.Add("api-key", apiKey);
                webRequest.Headers.Add("api-signature", signatureString);
            }

            try
            {
                if (postData != "")
                {
                    webRequest.ContentType = json ? "application/json" : "application/x-www-form-urlencoded";
                    var data = Encoding.UTF8.GetBytes(postData);
                    using (var stream = webRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    TimeLastQuery = Convert.ToDateTime(webResponse.Headers["Date"]).ToUniversalTime();
                    using (Stream str = webResponse.GetResponseStream())
                    using (StreamReader sr = new StreamReader(str))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    if (response == null)
                        throw;

                    TimeLastQuery = Convert.ToDateTime(response.Headers["Date"]).ToUniversalTime();

                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        //public List<OrderBookItem> GetOrderBook(string symbol, int depth)
        //{
        //    var param = new Dictionary<string, string>();
        //    param["symbol"] = symbol;
        //    param["depth"] = depth.ToString();
        //    string res = Query("GET", "/orderBook", param);
        //    return JsonSerializer.DeserializeFromString<List<OrderBookItem>>(res);
        //}
        public string GetPositions()
        {
            var param = new Dictionary<string, string>
            {
                ["reverse"] = true.ToString()
            };
            return Query("GET", "/position", param, true);
        }

        //Returns all positions made after given DateTime, format is "2018-10-22 12:45:37.123"
        public string GetPositions(DateTime dt)
        {
            var param = new Dictionary<string, string>
            {
                ["reverse"] = true.ToString(),
                ["startTime"] = dt.ToString()
            };
            return Query("GET", "/position", param, true);
        }

        public string GetWallets()
        {
            var param = new Dictionary<string, string>();
            return Query("GET", "/user/wallet", param, true);
        }
        public string GetChat()
        {
            var param = new Dictionary<string, string>
            {
                ["count"] = 100.ToString()
            };
            return Query("GET", "/chat", param, true);
        }

        public string GetOrders(string test)
        {
            var param = new Dictionary<string, string>
            {
                //param["symbol"] = "XBTUSD";
                //param["filter"] = "{\"open\":true}";
                //param["columns"] = "";
                //param["count"] = 100.ToString();
                //param["start"] = 0.ToString();
                ["reverse"] = true.ToString()
            };
            //param["startTime"] = "";
            //param["endTime"] = "";
            return Query("GET", "/order", param, true);
        }

        public IEnumerable<BitMEXOrder> ConvertToListOrders(string json)
        {
            // не дописан
            return null;
        }

        public IEnumerable<BitMEXOrder> GetListOrders(int count)
        { ///
            var param = new Dictionary<string, string>();
            //param["symbol"] = "XBTUSD";
            //param["filter"] = "{\"open\":true}";
            //param["columns"] = "";
            if (count > 0)
                param["count"] = count.ToString();
            //param["start"] = 0.ToString();
            param["reverse"] = true.ToString();
            //param["startTime"] = "";
            //param["endTime"] = "";
            return null /* Query("GET", "/order", param, true)*/ /*недописано!*/;
        }

        //Returns all orders made after given DateTime, format is "2018-10-22 12:45:37.123"
        public string GetOrders(DateTime dt)
        {
            var param = new Dictionary<string, string>
            {
                ["reverse"] = true.ToString(),
                ["startTime"] = dt.ToString()
            };
            return Query("GET", "/order", param, true);
        }
        public string PostOrders()
        {
            var param = new Dictionary<string, string>
            {
                ["symbol"] = "XBTUSD",
                ["side"] = "Buy",
                ["orderQty"] = "1",
                ["ordType"] = "Market"
            };
            return Query("POST", "/order", param, true);
        }

        public string DeleteOrders()
        {
            var param = new Dictionary<string, string>
            {
                ["orderID"] = "de709f12-2f24-9a36-b047-ab0ff090f0bb",
                ["text"] = "cancel order by ID"
            };
            return Query("DELETE", "/order", param, true, true);
        }

        private byte[] Hmacsha256(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA256(keyByte))
            {
                return hash.ComputeHash(messageBytes);
            }
        }

        #region RateLimiter

        private long lastTicks = 0;
        private int _countQuery;
        private readonly object thisLock = new object();

        private void RateLimit()
        {
            lock (thisLock)
            {
                long elapsedTicks = DateTime.Now.Ticks - lastTicks;
                var timespan = new TimeSpan(elapsedTicks);
                if (timespan.TotalMilliseconds < rateLimit)
                    Thread.Sleep(rateLimit - (int)timespan.TotalMilliseconds);
                lastTicks = DateTime.Now.Ticks;
            }
        }

        public BitMEXOrder LimitNowOrder(string Symbol, SideEnum hand, int Quantity, decimal Price/*, bool ReduceOnly = false, bool PostOnly = false, bool Hidden = false*/)
        {
            var param = new Dictionary<string, string>();
            param["symbol"] = Symbol;
            param["side"] = hand.ToString();
            param["orderQty"] = Quantity.ToString();
            param["ordType"] = "Limit";
            param["price"] = Price.ToString().Replace(",", ".");
            //if (ReduceOnly && !PostOnly)
            //{
            //    param["execInst"] = "ReduceOnly";
            //}
            //else if (!ReduceOnly && PostOnly)
            //{
            param["execInst"] = "ParticipateDoNotInitiate";
            //}
            //else if (ReduceOnly && PostOnly)
            //{
            //    param["execInst"] = "ReduceOnly,ParticipateDoNotInitiate";
            //}
            //if (Hidden)
            //{
            //    param["displayQty"] = "0";
            //}


            BitMEXOrder ret = null;
            int RetryAttemptCount = -1;
            int? MaxRetries = null;
            string res;
            while ((res = Query("POST", "/order", param, true)).Length < 0
                || RetryAttemptCount++ < (MaxRetries ?? (MaxRetries = RetryAttempts(res))))
            {
                //string res = Query("POST", "/order", param, true);
                if (res.Substring(0, 10).Contains("error"))
                {
                    Error.LastError = JsonConvert.DeserializeObject<RootError>(res).Error;
                    continue;
                }
                else if (res.TryParseXML(out XElement xRes))
                {
                    Error.LastError = new Error(xRes);
                    continue;
                }
                ret = JsonConvert.DeserializeObject<BitMEXOrder>(res);
                break;
            }
            //List<BitMEXOrder> ret = JsonConvert.DeserializeObject<List<BitMEXOrder>>(res);

            return ret;

            //while (res.Contains("error") && RetryAttemptCount < MaxRetries)
            //{
            //    errors.Add(res);
            //    Thread.Sleep(BitMEXAssistant.Properties.Settings.Default.RetryAttemptWaitTime); // Force app to wait 500ms
            //    res = Query("POST", "/order", param, true);
            //    RetryAttemptCount++;
            //    if (RetryAttemptCount == MaxRetries)
            //    {
            //        errors.Add("Max rety attempts of " + MaxRetries.ToString() + " reached.");
            //        break;
            //    }
            //}

            //try
            //{
            //    List<Order> Result = new List<Order>();
            //    Result.Add(JsonConvert.DeserializeObject<Order>(res));
            //    return Result;
            //}
            //catch (Exception ex)
            //{
            //    return new List<Order>();
            //}
        }

        public BitMEXOrder LimitNowAmendOrder(string OrderId, decimal? Price = null, int? OrderQty = null)
        {
            var param = new Dictionary<string, string>();
            param["orderID"] = OrderId;
            if (Price != null)
            {
                param["price"] = Price.ToString().Replace(",", ".");
            }
            if (OrderQty != null)
            {
                param["orderQty"] = OrderQty.ToString();
            }

            string res = Query("PUT", "/order", param, true);

            try
            {

                if (res.Substring(0, 10).Contains("error"))
                {
                    Error.LastError = JsonConvert.DeserializeObject<RootError>(res).Error;
                    return null;
                }
                else if (res.TryParseXML(out XElement xRes))
                {
                    Error.LastError = new Error(xRes);
                    return null;
                }
                BitMEXOrder ret = JsonConvert.DeserializeObject<BitMEXOrder>(res);
                return ret;

                //List<Order> Result = new List<Order>();
                //if (!res.Contains("error"))
                //{
                //    Result.Add(JsonConvert.DeserializeObject<Order>(res));
                //    return Result;
                //}
                //else
                //{
                //    return new List<Order>();
                //}

            }
            catch (Exception /*ex*/)
            {
                return null;
            }
        }


        #endregion RateLimiter
    }
}
