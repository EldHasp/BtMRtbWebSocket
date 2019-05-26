using CommLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BTMRTB
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public bool IsClose;
        static public List<Window> AllWindows = new List<Window>();
        //static public KeySecretWin KeySecret { get; private set; }
        //static public OrderBook10Win OrderBook10 { get; private set; }
        //static public TradeWin Trade { get; private set; }
        //static public CalcWin CalcView { get; private set; }
        static public void CloseAllWin()
        {
            IsClose = true;
            foreach (Window win in AllWindows)
            {
                win.Hide();
                win.Close();
            }
            AllWindows.Clear();
        }

        static public void CreateAllWin()
        {
            IsClose = false;

            AllWindows.Add(new KeySecretWin());
            AllWindows.Add(new TradeWin());
            //AllWindows.Add(new OrderBook10Win());
            AllWindows.Add(new CalcWin());
        }

        static public void OwnerAllWin(Window windowParent)
        {
            foreach (Window win in AllWindows)
                if (!object.ReferenceEquals(win, windowParent))
                    win.Owner = windowParent;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainDispatcher.dispatcher = Current.Dispatcher;
            MySetting.Load();
            CreateAllWin();
        }
    }
}
