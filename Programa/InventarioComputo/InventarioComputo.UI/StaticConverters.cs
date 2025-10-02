using System.Windows;
using System.Windows.Data;
using InventarioComputo.UI.Converters;

namespace InventarioComputo.UI
{
    public static class StaticConverters
    {
        // Crea instancias estáticas de cada convertidor
        public static BooleanToVisibilityConverter BooleanToVisibility { get; } = new BooleanToVisibilityConverter();
        public static InvertedBooleanToVisibilityConverter InvertedBooleanToVisibility { get; } = new InvertedBooleanToVisibilityConverter();
        public static MultiAndBoolToVisibilityConverter MultiAndBoolToVisibility { get; } = new MultiAndBoolToVisibilityConverter();
        public static BoolToStringConverter BoolToString { get; } = new BoolToStringConverter();
        public static BoolToStatusText BoolToStatusText { get; } = new BoolToStatusText();
        public static BoolToActivoConverter BoolToActivo { get; } = new BoolToActivoConverter();
        public static ActiveStatusConverter ActiveStatus { get; } = new ActiveStatusConverter();
        public static StatusColorConverter StatusColor { get; } = new StatusColorConverter();
        public static BoolToStatusBrush BoolToStatusBrush { get; } = new BoolToStatusBrush();
        public static MenuWidthConverter MenuWidth { get; } = new MenuWidthConverter();
        public static ColorHexToBrushConverter ColorHexToBrush { get; } = new ColorHexToBrushConverter();
        public static StringToColorConverter StringToColor { get; } = new StringToColorConverter();
        public static NullToBoolConverter NullToBool { get; } = new NullToBoolConverter();
        public static NotNullToBoolConverter NotNullToBool { get; } = new NotNullToBoolConverter();
        public static InvertBooleanConverter InvertBoolean { get; } = new InvertBooleanConverter();
        public static InverseBooleanConverter InverseBoolean { get; } = new InverseBooleanConverter();
    }
}