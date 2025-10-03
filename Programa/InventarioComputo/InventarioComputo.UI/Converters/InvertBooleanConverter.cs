// InventarioComputo.UI\Converters\InvertBooleanConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace InventarioComputo.UI.Converters
{
    public class InvertBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;
    }
}