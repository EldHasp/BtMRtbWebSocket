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
    public partial class ViewModelWSOrderBook10 : ViewModelWSOrderBook10DD
    {
        static public ViewModelWSOrderBook10 ViewModelWS { get; private set; }

        private SettingsClass _settings = MySetting.Settings;
        private WebSocketBitMexUnSigned _wSocket;
        public WebSocketBitMexUnSigned WSocket { get => _wSocket;private set { SetProperty(ref _wSocket, value); } }
        public SettingsClass Settings { get => _settings; private set { SetProperty(ref _settings, value); } }

        public ViewModelWSOrderBook10()
        {
            if (ViewModelWS != default)
                throw new Exception("Повторное создание WebSocket без авторизации");
            ViewModelWS = this;

            WSocket = WebSocketBitMexUnSigned.Create(Settings.RealWork);

            WSocket.PropertyChanged += WSocket_PropertyChangedAsync;
            WSocket.OnAllPropertyChanged();
            //Settings = MySetting.Settings;
            //Settings.PropertyChanged += Settings_PropertyChanged;
            //Settings.SPropertyChanged += MySetting_ChangeSettings;

        }

        //private void MySetting_ChangeSettings(object sender, PropertyChangedEventArgs e)
        //{
        //    string propertyName = e.PropertyName?.Trim();
        //    if (string.IsNullOrWhiteSpace(propertyName) || propertyName == "Settings")
        //        Settings = Settings.settings;
        //}


        //protected override void PropertyNewValue<T>(ref T fieldProperty, T newValue, string nameProperty)
        //{

        //    base.PropertyNewValue(ref fieldProperty, newValue, nameProperty);

        //    if (nameProperty == "Settings" && newValue is Settings settings)
        //        settings.PropertyChanged += Settings_PropertyChanged;

        //    if (nameProperty == "WSocket" && newValue is WebSocketBitMexUnSigned ws)
        //        ws.PropertyChanged += WSocket_PropertyChangedAsync;
        //}

        //private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    string propertyName = e.PropertyName?.Trim();

        //    if (string.IsNullOrWhiteSpace(propertyName) || propertyName == "RealWork")
        //    {
        //        if (WSocket != null)
        //        {
        //            if (WSocket.IsOpen)
        //                WSocket.WS.Close();
        //            WSocket = null;
        //        }
        //        if (Settings != null)
        //            WSocket = new WebSocketBitMexUnSigned(Settings.RealWork);
        //    }

        //}
    }
}
