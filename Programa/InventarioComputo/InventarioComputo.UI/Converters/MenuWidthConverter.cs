using System;
using System.Globalization;
using System.Windows.Data;

namespace InventarioComputo.UI.Converters
{
    public class MenuWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpanded && parameter is string widthsString)
            {
                string[] widths = widthsString.Split(',');
                if (widths.Length >= 2)
                {
                    return isExpanded ? double.Parse(widths[0], culture) : double.Parse(widths[1], culture);
                }
            }
            return 200;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}