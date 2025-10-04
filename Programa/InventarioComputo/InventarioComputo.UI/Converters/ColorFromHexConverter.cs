﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace InventarioComputo.UI.Converters
{
    public class ColorFromHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hex && !string.IsNullOrWhiteSpace(hex))
            {
                try
                {
                    return (Color)ColorConverter.ConvertFromString(hex);
                }
                catch
                {
                    return Colors.Gray;
                }
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}