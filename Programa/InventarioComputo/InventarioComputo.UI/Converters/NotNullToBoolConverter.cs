// InventarioComputo.UI\Converters\NotNullToBoolConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace InventarioComputo.UI.Converters
{
    public class NotNullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value != null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}