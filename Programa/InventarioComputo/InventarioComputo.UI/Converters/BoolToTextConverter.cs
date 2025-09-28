// InventarioComputo.UI\Converters\BoolToTextConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace InventarioComputo.UI.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                string[] options = parameter?.ToString()?.Split('|') ?? new[] { "Sí", "No" };
                return boolValue ? options[0] : options.Length > 1 ? options[1] : "No";
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}