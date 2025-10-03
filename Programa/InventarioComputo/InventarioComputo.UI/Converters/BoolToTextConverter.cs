// InventarioComputo.UI\Converters\BoolToTextConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace InventarioComputo.UI.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? (b ? "Sí" : "No") : "N/A";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}