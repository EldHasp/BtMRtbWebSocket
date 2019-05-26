using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WebSocketSharp;

namespace BitMexLibrary
{
    public partial class WebSocketBitMexSigned : OnPropertyChangedClass
    {

        #region Событие SPropertyChanged
        /// <summary>Событие для извещения об изменения свойства</summary>
        private static SPropertyChangedEventHandler _spropertyChanged;
        public static event SPropertyChangedEventHandler SPropertyChanged
        {
            add
            {
                if (_spropertyChanged == null)
                    _spropertyChanged += value;
                else
                    lock (_spropertyChanged)
                        _spropertyChanged += value;
                value(typeof(SettingsClass), new PropertyChangedEventArgs(null));
            }
            remove { lock (_spropertyChanged) { _spropertyChanged -= value; } }
        }

        /// <summary>Метод для вызова события извещения об изменении свойства</summary>
        /// <param name="prop">Изменившееся свойство</param>
        public static void OnSPropertyChanged([CallerMemberName]string prop = "")
            => _spropertyChanged?.Invoke(typeof(WebSocketBitMexSigned), new PropertyChangedEventArgs(prop));

        #endregion

        public void Close()
        {
            ws.OnClose -= Ws_OnClose;
            ws.OnError -= Ws_OnError;
            ws.OnMessage -= Ws_OnMessageAsync;
            ws.OnOpen -= Ws_OnOpen;

            ws.Close();
            IsOpen = false;
            IsClose = true;
            Authorization = null;
            ws = null;
        }

        const string UrlBitMexReal = "wss://www.bitmex.com/realtime";
        const string UrlBitMexTest = "wss://testnet.bitmex.com/realtime";
        WebSocket ws;

        public WebSocketBitMexSigned(string APIKey = "", string APISecret = "", bool RealWork = false)
        {
            WSocket = this;
            RealNet = RealWork;
            ws = new WebSocket(
                                (RealNet ? UrlBitMexReal : UrlBitMexTest)
                                //+ "?subscribe=orderBook10:" + WorkSymbol
                              );

            ws.OnClose += Ws_OnClose;
            ws.OnError += Ws_OnError;
            ws.OnMessage += Ws_OnMessageAsync;
            ws.OnOpen += Ws_OnOpen;

            ws.Connect();

            // Аутентифицировать API
            //long APIExpires = GetExpiresArg(); // количество секудн от 01.01.70г. до времени истечения срока
            // Строка шестнадцатеричного представления массива байт полученного Хеш от 'GET/realtime' + APIExpires по ключу APISecret
            //string Signature = GetWebSocketSignatureString(APISecret, APIExpires);

            // Передача сообщения серверу
            //SendOpAuthKeyExpires sendOpAuth = new SendOpAuthKeyExpires()
            //{
            //    APIKey = APIKey,
            //    APIExpires = APIExpires,
            //    Signature = Signature
            //};

            //string wsSend = JsonConvert.SerializeObject(sendOpAuth);
            //"{\"op\": \"authKeyExpires\", \"args\": [\"" + APIKey + "\", " + APIExpires + ", \"" + Signature + "\"]}";

            string wsSend = SendOpSrting.AuthKeyExpires(APIKey, APISecret);
            Console.WriteLine($"Send=\"{wsSend}\"");
            ws.Send(wsSend);
            //          Console.WriteLine($"Send=\"{wsSend}\"");


            //wsSend = "\"help\"";
            wsSend = SendOpSrting.Help;
            Console.WriteLine($"Send=\"{wsSend}\"");
            ws.Send(wsSend);

            //wsSend = "{\"op\": \"subscribe\", \"args\": [\"margin\"]}";
            //wsSend = JsonConvert.SerializeObject(SendOp.Margin);
            wsSend = SendOpSrting.Margin;
            Console.WriteLine($"Send=\"{wsSend}\"");
            ws.Send(wsSend);


            //wsSend = "{\"op\": \"subscribe\", \"args\": [\"wallet\"]}";
            //wsSend = JsonConvert.SerializeObject(SendOp.Wallet);
            wsSend = SendOpSrting.Wallet;
            Console.WriteLine($"Send=\"{wsSend}\"");
            ws.Send(wsSend);

            // Subscribe to new orderbook
            //Thread.Sleep(1000);
            //SendOpSubscribe sendOpSubsc = new SendOpSubscribe()
            //{
            //    Name = "orderBook10",
            //    Symbol = WorkSymbol
            //};

            //wsSend = JsonConvert.SerializeObject(sendOpSubsc);
            //Console.WriteLine($"Send=\"{wsSend}\"");
            //ws.Send(wsSend);
        }

        public void SubscribeAdditional()
        {
            string wsSend;
            wsSend = SendOpSrting.Position;
            Console.WriteLine($"Send=\"{wsSend}\"");
            //wsSend = "{\"op\": \"subscribe\", \"args\": [\"position:" + WorkSymbol + "\"]}";
            ws.Send(wsSend);

            wsSend = SendOpSrting.Order;
            Console.WriteLine($"Send=\"{wsSend}\"");
            ws.Send(wsSend);

            //wsSend = SendOpSrting.Order;
            wsSend = "{\"op\": \"subscribe\", \"args\": [\"quote:" + WorkSymbol + "\"]}";
            Console.WriteLine($"Send=\"{wsSend}\"");
            ws.Send(wsSend);
        }

    }
}
