using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CommLibrary.Converters
{
    [ValueConversion(typeof(bool?), typeof(string))]
    public class NullableBoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            // Проверка и преобразование входного значения
            bool? val;
            if (value == null)
                val = null;
            else if (value is bool valBool)
                val = valBool;
            else if (value is string valStr)
                val = bool.Parse(valStr);
            else
                throw new ArgumentException();

            // Проверка и преобразование параметра
            string[] parArr=null;
            if (parameter is IEnumerable<string> parIE)
                parArr = parIE.ToArray();
            else if (parameter is string parStr)
                parArr = parStr.Split("\r ,\n\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parArr == null || parArr.Length == 0)
                return val == null ? "Null" : val.Value.ToString();
            switch (parArr.Length)
            {
                case 1: return val == true ? parArr[0] : "";
                case 2: return val == true ? parArr[0] : val == false ? parArr[1] : "";
                default: return val == true ? parArr[0] : val == false ? parArr[1] : parArr[2];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Обратная ковертация не реализована");
        }
    }
}
