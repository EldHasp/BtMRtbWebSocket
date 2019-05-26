using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CommLibrary.Converters
{
    /// <summary>Конвертер преобразующий bool в Visibility</summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public string NameS { get; set; } = null;

        /// <summary>Прямая конвертация bool в Visibility</summary>
        /// <param name="value">Значение для конвертации</param>
        /// <param name="targetType">Целевой тип</param>
        /// <param name="parameter">Параметр. Если в нём "not", "no", "false" или "нет", то инвертируем видимость. Если "hide", "hidden" или "скрыть", то вместо колапса - скрытие</param>
        /// <param name="culture">Не используется</param>
        /// <returns>Видимость</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility ret = Visibility.Visible;

            // Если проверяемое значение не null и равно false, то коллапс
            if ((value is bool valBool && !valBool)
                || (value is string valStr && valStr.ToLower().Contains("false"))
               ) ret = Visibility.Collapsed;

            // Если параметр не null, то проверяем его тип
            if (parameter != null)
            {
                // Если тип параметра bool и он равен false, то инвертируем видимость
                if (parameter is bool parBool)
                {
                    if (!parBool)
                        ret = ret == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                }
                // Если тип параметра string и он содержит "not", "no", "false" или "нет", то инвертируем видимость
                else if (parameter is string parStr)
                {
                    // если содержит "not", "no", "false" или "нет", то инвертируем видимость
                    if (!string.IsNullOrWhiteSpace("not no false нет".Split().FirstOrDefault(str => ((string)parameter).ToLower().Contains(str))))
                        ret = ret == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

                    // если содержит "hidden" или "скрыть", то заменяем Collapsed на Hidden
                    if (!string.IsNullOrWhiteSpace("hide hidden скрыть".Split().FirstOrDefault(str => ((string)parameter).ToLower().Contains(str))))
                        if (ret == Visibility.Collapsed)
                            ret = Visibility.Hidden;

                }

                // Если тип параметра Visibility и он равен Hidden, то заменяем Collapsed на Hidden
                else if (parameter is Visibility parVisib)
                {
                    if (parVisib == Visibility.Hidden && ret == Visibility.Collapsed)
                        ret = Visibility.Hidden;
                }
            }
            return ret;
        }

        /// <summary>Обратная конвертация</summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Если проверяемое значение null или его тип не Visibility,
            // или тип строковй но его нельзя преобразовать в Visibility,
            // то выход без конвертации
            if (value == null
                || !(value is Visibility val)
                || (value is string valStr && Enum.TryParse(valStr, out val)))
                return value;

            bool ret = val == Visibility.Visible;

            // Если параметр не null, то проверяем его тип
            if (parameter != null)
            {
                Type parType = parameter.GetType();
                // Если тип параметра bool и он равен false, то инвертируем значение
                if (parType == typeof(bool) && (bool)parameter == false)
                    ret = !ret;

                // Если тип параметра string и он содержит "not", "no", "false" или "нет", то инвертируем значение
                if (parType == typeof(string)
                    && !string.IsNullOrWhiteSpace("not no false нет".Split().FirstOrDefault(str => ((string)parameter).ToLower().Contains(str))))
                    ret = !ret;
            }
            return ret;
        }
    }
}
