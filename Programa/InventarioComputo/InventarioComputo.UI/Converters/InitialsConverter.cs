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
            if (value is string name && !string.IsNullOrWhiteSpace(name))
            {
                // Dividir el nombre por espacios y tomar la inicial de cada palabra
                var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (parts.Length > 0)
                {
                    // Si hay más de una palabra, tomar la inicial de la primera y última
                    if (parts.Length > 1)
                    {
                        return $"{parts.First().Substring(0, 1)}{parts.Last().Substring(0, 1)}".ToUpper();
                    }
                    
                    // Si solo hay una palabra, tomar las dos primeras letras o la primera si es muy corta
                    return parts[0].Length > 1 ? 
                        parts[0].Substring(0, 2).ToUpper() : 
                        parts[0].Substring(0, 1).ToUpper();
                }
            }
            
            return "U";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}