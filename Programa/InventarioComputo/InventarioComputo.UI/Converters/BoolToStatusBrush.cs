using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace InventarioComputo.UI.Converters
{
    public class BoolToStatusBrush : IValueConverter
    {
        private static readonly SolidColorBrush Active = new SolidColorBrush(Colors.Green);
        private static readonly SolidColorBrush Inactive = new SolidColorBrush(Colors.Red);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b && b ? Active : Inactive;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}