using BitMexLibrary.WebSocketJSON;
using System.Windows;
using System.Windows.Data;
using ViewModel;

namespace BTMRTB
{
    /// <summary>
    /// Логика взаимодействия для TradeWin.xaml
    /// </summary>
    public partial class TradeWin : Window
    {
        private static ViewModelTrade viewModelTrade;
        public TradeWin()
        {
            InitializeComponent();

            if (viewModelTrade == default)
                viewModelTrade = ViewModelTrade.ViewModel;
            DataContext = viewModelTrade;
            //viewModelTrade.PropertyChanged += ViewModelTrade_PropertyChanged;

        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is Position position)
                e.Accepted = position.IsActive;
        }

        //private void ViewModelTrade_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    string propName = e.PropertyName;
        //    if (string.IsNullOrWhiteSpace("AuthorizationComplete") || propName == "AuthorizationComplete")
        //        if (viewModelTrade.AuthorizationComplete)
        //            Show();
        //        else
        //            Hide();
        //}
    }
}
