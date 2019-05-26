using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CommLibrary.Converters
{
    /// <summary>
    /// Извлекает компоненты <see langword="DateTime"/>.
    /// Если значение было <see langword="null"/>, то возвращает <see langword="null"/>
    /// Конвертер ещё не реализован
    /// </summary>
    [ValueConversion(typeof(DateTime?), typeof(DateTime))]
    public class DateTimeExtractConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => value == null ? Visibility.Hidden : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
