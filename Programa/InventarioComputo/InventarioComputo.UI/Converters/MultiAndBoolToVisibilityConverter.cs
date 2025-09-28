// InventarioComputo.UI\Converters\MultiAndBoolToVisibilityConverter.cs
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InventarioComputo.UI.Converters
{
    public class MultiAndBoolToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Si todos los valores son true, retorna Visible
            bool allTrue = values != null && values.All(v => v is bool b && b);
            return allTrue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}