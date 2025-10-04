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
                var names = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (names.Length >= 2)
                {
                    // Tomar la inicial del primer y último nombre
                    return $"{names[0][0]}{names[names.Length - 1][0]}".ToUpper();
                }
                else if (names.Length == 1)
                {
                    // Si solo hay un nombre, tomar las dos primeras letras o repetir la primera
                    return names[0].Length > 1 
                        ? $"{names[0][0]}{names[0][1]}".ToUpper() 
                        : $"{names[0][0]}{names[0][0]}".ToUpper();
                }
            }
            
            return "??";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}