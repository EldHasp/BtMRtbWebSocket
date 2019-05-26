﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CommLibrary.Converters
{
    /// <summary>
    /// Если значение <see langword="null"/> возвращает <see langword="Visibility.Hidden"/>,
    /// иначе <see langword="Visibility.Visible"/>
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => value == null ? Visibility.Hidden : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
