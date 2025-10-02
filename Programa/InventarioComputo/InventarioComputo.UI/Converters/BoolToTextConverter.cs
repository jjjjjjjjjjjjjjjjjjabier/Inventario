// InventarioComputo.UI\Converters\BoolToTextConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace InventarioComputo.UI.Converters
{
    // Convierte bool a texto. Soporta parámetro "TrueText|FalseText" (opcional).
    // Ej: ConverterParameter="Administrador|Consulta"
    public class BoolToTextConverter : IValueConverter
    {
        public string TrueText { get; set; } = "Sí";
        public string FalseText { get; set; } = "No";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Permite sobreescribir por parámetro: "TextoParaTrue|TextoParaFalse"
            if (parameter is string p && !string.IsNullOrWhiteSpace(p))
            {
                var parts = p.Split('|');
                if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0])) TrueText = parts[0];
                if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1])) FalseText = parts[1];
            }

            return value is bool b ? (b ? TrueText : FalseText) : FalseText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Opcional: interpreta de vuelta comparando con los textos configurados
            if (value is string s)
            {
                if (string.Equals(s, TrueText, StringComparison.CurrentCultureIgnoreCase)) return true;
                if (string.Equals(s, FalseText, StringComparison.CurrentCultureIgnoreCase)) return false;
            }
            return false;
        }
    }
}