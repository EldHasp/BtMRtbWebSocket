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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfLibrary
{
    /// <summary>
    /// Логика взаимодействия для WindowHS.xaml
    /// </summary>
    public class WindowHS : Window
    {
        public WindowHS() { IsHide = Visibility != Visibility.Visible; }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name == "Visibility")
                IsHide = Visibility != Visibility.Visible;
        }



        /// <summary>Свойство для привязки срытия окна</summary>
        public bool IsHide
        {
            get { return (bool)GetValue(IsHidetProperty); }
            set { SetValue(IsHidetProperty, value); }
        }
        public static readonly DependencyProperty IsHidetProperty =
            DependencyProperty.Register("IsHide", typeof(bool), typeof(WindowHS), new PropertyMetadata(true, PropChange));

        private static void PropChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window win && e.NewValue is bool newIsHide && e.OldValue is bool oldIsHide && newIsHide != oldIsHide)
                if (newIsHide)
                { if (win.Visibility == Visibility.Visible) win.Hide(); }
                else
                { if (win.Visibility != Visibility.Visible) win.Show(); }
        }
    }
}
