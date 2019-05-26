using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CommLibrary.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value is bool valBool)
                return !valBool;
            if (value is string valStr)
                return !bool.Parse(valStr);
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value is bool valBool)
                return !valBool;
            if (value is string valStr)
                return !bool.Parse(valStr);
            throw new ArgumentException();
        }
    }
}
