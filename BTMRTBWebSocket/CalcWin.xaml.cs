using CommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ViewModel;

namespace BTMRTB
{
    /// <summary>
    /// Логика взаимодействия для KeySecret.xaml
    /// </summary>
    public partial class CalcWin : Window
    {
        private static ViewModelTrade viewModelTrade;

        public CalcWin()
        {
            InitializeComponent();
            if (viewModelTrade == default)
                viewModelTrade = ViewModelTrade.ViewModel;
            DataContext = viewModelTrade;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!App.IsClose)
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tmp = dataGrid.ItemsSource;
            var ttmp = dataGrid.Items;
            var row = ((IEnumerable<object>)tmp).Last();

        }
    }
}
