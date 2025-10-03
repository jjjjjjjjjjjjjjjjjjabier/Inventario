using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace InventarioComputo.UI.Converters
{
    public class ColorHexToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hex = value as string;
            if (string.IsNullOrWhiteSpace(hex)) return Brushes.Black;
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex)!;
                return new SolidColorBrush(color);
            }
            catch
            {
                return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}