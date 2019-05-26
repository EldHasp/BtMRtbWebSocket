using CommLibrary;
using BitMexLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public partial class ViewModelTrade : ViewModelTradeDD
    {
        private static ViewModelTrade _viewModel;
        public static ViewModelTrade ViewModel => _viewModel ?? (_viewModel = new ViewModelTrade());

        //public static WebSocketBitMexSigned WSocket { get; private set; }

        public void Open(RESTBitMexSigned bitMEX, WebSocketBitMexSigned wSocket)
        {
            Settings = MySetting.Settings;

            bitMexREST = bitMEX;

            WebSocketSigned = wSocket;

            WebSocketSigned.PropertyChanged += WebSocketSigned_PropertyChangedAsync;
            WebSocketSigned.OnAllPropertyChanged();
            WebSocketSigned.SubscribeAdditional();

            //WebSocketUnSigned = ViewModelWSOrderBook10.ViewModelWS.WSocket;
            //WebSocketUnSigned.PropertyChanged += WebSocketUnSigned_PropertyChanged;
            //WebSocketUnSigned.OnAllPropertyChanged();

            //WSocket = WebSocketBitMexSigned.WSocket;
            AuthorizationComplete = true;
        }
    }
}
