// InventarioComputo.UI\Converters\BoolToTextConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace InventarioComputo.UI.Converters
{
    // Convierte bool a texto. Soporta ConverterParameter con formato "TrueText|FalseText"
    public class BoolToTextConverter : IValueConverter
    {
        public string TrueText { get; set; } = "Sí";
        public string FalseText { get; set; } = "No";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var (trueText, falseText) = ParseParam(parameter);
            if (value is bool b)
                return b ? trueText : falseText;

            return falseText; // fallback para valores no booleanos
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();

        private (string trueText, string falseText) ParseParam(object parameter)
        {
            if (parameter is string s)
            {
                var parts = s.Split('|');
                if (parts.Length >= 2)
                    return (parts[0], parts[1]);
            }
            return (TrueText, FalseText);
        }
    }
}