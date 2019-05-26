using BitMexLibrary;
using CommLibrary;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace ViewModel
{
    public partial class ViewModelAuth : ViewModelAuthDD
    {


        protected override void OnAuthorReq(object param)
        {
            AuthorizationRequest = true;
            MySetting.Save();
            BitMexREST = new RESTBitMexSigned(Settings.APIKey, Settings.APISecret, Settings.RealWork);
            BitMexREST.PropertyChanged += BitMexREST_PropertyChanged;
            BitMexREST.OnAllPropertyChanged();
            bitMexWebSocket = new WebSocketBitMexSigned(Settings.APIKey, Settings.APISecret, Settings.RealWork);
            bitMexWebSocket.PropertyChanged += BitMexWebSocket_PropertyChangedAsync;
            bitMexWebSocket.OnAllPropertyChanged();

        }

        protected override void OnReAuthorReq(object param)
        {
            BitMexREST.PropertyChanged -= BitMexREST_PropertyChanged;
            BitMexREST = default;
            bitMexWebSocket.PropertyChanged -= BitMexWebSocket_PropertyChangedAsync;
            bitMexWebSocket.Close();
            bitMexWebSocket = null;
            AuthorizationRequest = false;
            OnAllPropertyChanged();
        }

        protected override void OnContinue(object param)
        {
            AuthorizationComplete = true;
            ViewModelTrade.ViewModel.Open(BitMexREST, bitMexWebSocket);
        }


        public ViewModelAuth()
        {
            Settings = MySetting.Settings;
        }

    }
}
