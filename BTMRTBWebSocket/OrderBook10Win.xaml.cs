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
    /// Логика взаимодействия для OrderBook10Win.xaml
    /// </summary>
    public partial class OrderBook10Win : Window
    {
        public static ViewModelWSOrderBook10 ViewModelBook10 { get; private set; } = new ViewModelWSOrderBook10();
        public OrderBook10Win()
        {
            InitializeComponent();
            //if (ViewModelBook10 == default)
            //    ViewModelBook10 = new ViewModelWSOrderBook10();
            DataContext = ViewModelBook10;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!App.IsClose)
            {
                e.Cancel = true;
                WindowState = WindowState.Minimized;
            }
        }
    }
}
