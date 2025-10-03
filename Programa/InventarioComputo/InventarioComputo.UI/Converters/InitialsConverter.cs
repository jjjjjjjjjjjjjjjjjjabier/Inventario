using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace InventarioComputo.UI.Converters
{
    public class InitialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = value as string ?? string.Empty;
            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return string.Empty;
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper(culture);
            return string.Concat(parts[0].FirstOrDefault(), parts[1].FirstOrDefault()).ToUpper(culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}