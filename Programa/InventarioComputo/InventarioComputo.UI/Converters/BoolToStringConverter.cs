using System;
using System.Globalization;
using System.Windows.Data;

namespace InventarioComputo.UI.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueText { get; set; } = "Sí";
        public string FalseText { get; set; } = "No";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? (b ? TrueText : FalseText) : FalseText;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (string.Equals(s, TrueText, StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(s, FalseText, StringComparison.OrdinalIgnoreCase)) return false;
            return false;
        }
    }
}