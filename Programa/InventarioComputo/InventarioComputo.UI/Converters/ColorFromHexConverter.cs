using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace InventarioComputo.UI.Converters
{
    /// <summary>
    /// Convierte un código de color hexadecimal (ej. "#FF0000") a un objeto Brush de WPF.
    /// </summary>
    public class ColorFromHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hexColor && !string.IsNullOrWhiteSpace(hexColor))
            {
                try
                {
                    // CORRECCIÓN: Se crea y devuelve un SolidColorBrush, que es lo que la UI necesita.
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
                }
                catch
                {
                    // Si el color es inválido, devuelve un pincel transparente.
                    return Brushes.Transparent;
                }
            }
            // Si el valor es nulo o vacío, también devuelve transparente.
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // No necesitamos esta dirección de conversión.
            throw new NotImplementedException();
        }
    }
}