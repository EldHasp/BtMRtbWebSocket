using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace BitMexLibrary
{
    public partial class WebSocketBitMexUnSigned
    {
        public static WebSocketBitMexUnSigned WSBitMexUnSigned { get; set; }

        //public static WebSocketBitMexUnSigned WSocket { get; private set; }
        public static WebSocketBitMexUnSigned Create(bool RealNet)
        {
            if (WSBitMexUnSigned == default)
                WSBitMexUnSigned = new WebSocketBitMexUnSigned(RealNet);
            else if (WSBitMexUnSigned.RealNet != RealNet)
                throw new Exception("Повтороное создание WebSocket без авторизации");
            return WSBitMexUnSigned;
        }

    }
}
