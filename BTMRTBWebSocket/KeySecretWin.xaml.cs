using System.Windows;
using ViewModel;

namespace BTMRTB
{
    /// <summary>
    /// Логика взаимодействия для KeySecret.xaml
    /// </summary>
    public partial class KeySecretWin : Window
    {
        private static ViewModelAuth viewModelAuth;
        public KeySecretWin()
        {
            InitializeComponent();

            if (viewModelAuth == default)
                viewModelAuth = new ViewModelAuth();
            //viewModelAuth.PropertyChanged += ViewModelAuth_PropertyChanged;
            DataContext = viewModelAuth;

            PassKey.Password = viewModelAuth.Settings.APIKey;
            PassSecret.Password = viewModelAuth.Settings.APISecret;

            
        }


        //private void ViewModelAuth_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    string propName = e.PropertyName;
        //    if (string.IsNullOrWhiteSpace("AuthorizationComplete") || propName == "AuthorizationComplete")
        //        if (viewModelAuth.AuthorizationComplete)
        //            Hide();
        //        else
        //            Show();
        //}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (App.IsClose)
                return;

            App.CloseAllWin();
        }

        private void PassKey_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModelAuth.Settings.APIKey = PassKey.Password;
        }
        private void PassSecret_PasswordChanged(object sender, RoutedEventArgs e)
        {
            viewModelAuth.Settings.APISecret = PassSecret.Password;
        }

        private void PassKey_GotFocus(object sender, RoutedEventArgs e)
        {
            PassKey.Password = viewModelAuth.Settings.APIKey;

        }

        private void PassSecret_GotFocus(object sender, RoutedEventArgs e)
        {
            PassSecret.Password = viewModelAuth.Settings.APISecret;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            checkBoxPass.IsChecked = false;
        }

        bool neverShowed = true;

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (neverShowed && Visibility == Visibility.Visible)
            {
                App.OwnerAllWin(this);
                neverShowed = false;
            }
        }

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    Hide();
        //}
    }
}
