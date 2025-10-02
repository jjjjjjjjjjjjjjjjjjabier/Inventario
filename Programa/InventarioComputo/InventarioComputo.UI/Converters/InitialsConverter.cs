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
            if (value is string fullName && !string.IsNullOrWhiteSpace(fullName))
            {
                // Dividir el nombre en palabras
                var words = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                // Tomar la primera letra de hasta dos palabras
                if (words.Length >= 2)
                {
                    return $"{words[0][0]}{words[1][0]}".ToUpper();
                }
                else if (words.Length == 1 && words[0].Length > 0)
                {
                    return words[0][0].ToString().ToUpper();
                }
            }
            
            // Valor por defecto si no se puede obtener iniciales
            return "?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("La conversión inversa no está soportada para InitialsConverter");
        }
    }
}