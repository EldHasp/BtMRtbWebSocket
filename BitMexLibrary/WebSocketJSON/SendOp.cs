using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BitMexLibrary;
using CF = BitMexLibrary.CommonFunction;

namespace BitMexLibrary.WebSocketJSON
{
    public class SendOpAuthKeyExpires : OpClass
    {
        private new string Op { get => base.Op; set => base.Op = value; }
        private new object[] Args { get => base.Args; set => base.Args = value; }

        public SendOpAuthKeyExpires()
        {
            Op = "authKeyExpires";
            Args = new object[] { "", 0, "" };
        }

        public string APIKey { get => (string)Args[0]; set => Args[0] = value; }
        public long APIExpires { get => (long)Args[1]; set => Args[1] = value; }
        public string Signature { get => (string)Args[2]; set => Args[2] = value; }

    }

    public class SendOpSubscribe : OpClass
    {

        public SendOpSubscribe()
        {
            Op = "subscribe";
            Args = new object[] { ":" };
        }

        public string Name { get => ((string)Args[0]).Split(':')[0]; set => Args[0] = $"{value.Trim()}:{Symbol.Trim()}"; }
        public string Symbol { get => ((string)Args[0]).Split(':')[1]; set => Args[0] = $"{Name.Trim()}:{value.Trim()}"; }
    }
    public class SendOpSubscribeKey : OpClassDict
    {
        KeyValuePair<string, object> KeyValue => Args.First();
        public SendOpSubscribeKey()
        {
            Op = "subscribe";
            Args = new Dictionary<string, object>();
        }
        public SendOpSubscribeKey(string key, object value)
        {
            Op = "subscribe";
            Args = new Dictionary<string, object>() { { key, value } };
        }
        public string Key
        {
            get => KeyValue.Key;
            set
            {
                if (value != KeyValue.Key)
                {
                    object _val = KeyValue.Value;
                    Args.Remove(KeyValue.Key);
                    Args.Add(value, _val);
                }
            }
        }
        public object Value
        {
            get => KeyValue.Value;
            set
            {
                if (value != KeyValue.Value)
                {
                    string _key = KeyValue.Key;
                    Args.Remove(KeyValue.Key);
                    Args.Add(_key, value);
                }
            }
        }
    }

    public static class SendOp
    {
        /// <summary>Создаёт экземпляр OpClass с Op = "subscribe" и одним строковым Args</summary>
        /// <param name="nameSubscribe">Имя одного строкового Args</param>
        /// <returns>Созданный экземпляр OpClass</returns>
        public static OpClass OpClassArgsString(string nameSubscribe) => new OpClass() { Op = "subscribe", Args = new object[] { nameSubscribe } };
        /// <summary>Создаёт экземпляр OpClass с Op = "subscribe" и одним строковым Args</summary>
        /// <param name="nameSubscribe">Имя одного строкового Args</param>
        /// <param name="valueSubscribe">Значение одного строкового Args</param>
        /// <returns>Созданный экземпляр OpClass</returns>
        public static OpClass OpClassArgsValueString(string nameSubscribe, string valueSubscribe)
            => new OpClass() { Op = "subscribe", Args = new object[] { nameSubscribe, valueSubscribe } };
        /// <summary>"{\"op\": \"subscribe\", \"args\": [\"wallet\"]}"</summary>
        public static OpClass Wallet => OpClassArgsString("wallet");
        /// <summary>"{\"op\": \"subscribe\", \"args\": [\"margin\"]}"</summary>
        public static OpClass Margin => OpClassArgsString("margin");
        /// <summary>"{\"op\": \"subscribe\", \"args\": [\"position\"]}"</summary>
        public static OpClass Position => OpClassArgsString("position");
        /// <summary>"{\"op\": \"subscribe\", \"args\": [\"order\"]}"</summary>
        public static OpClass Order => OpClassArgsString("order");
    }

    public static class SendOpSrting
    {
        public static string Help => "\"help\"";
        public static string Wallet => JsonConvert.SerializeObject(SendOp.Wallet);
        public static string Margin => JsonConvert.SerializeObject(SendOp.Margin);
        public static string Position => JsonConvert.SerializeObject(SendOp.Position);
        public static string Order => JsonConvert.SerializeObject(SendOp.Order);
        public static string AuthKeyExpires(string APIKey, string APISecret)
        {
            // Аутентифицировать API
            long APIExpires = CF.GetExpiresArg(); // количество секудн от 01.01.70г. до времени истечения срока
            // Строка шестнадцатеричного представления массива байт полученного Хеш от 'GET/realtime' + APIExpires по ключу APISecret
            string Signature = CF.GetWebSocketSignatureString(APISecret, APIExpires);

            return JsonConvert.SerializeObject(new SendOpAuthKeyExpires()
            {
                APIKey = APIKey,
                APIExpires = APIExpires,
                Signature = Signature
            });
        }
    }
}
