using System;
using System.Globalization;
using System.Windows.Data;
using InventarioComputo.Domain.Entities;

namespace InventarioComputo.UI.Converters
{
    public class MovimientoTipoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mov = value as HistorialMovimiento;
            if (mov == null) return "Movimiento";

            bool cambioEmpleado = mov.EmpleadoAnteriorId != mov.EmpleadoNuevoId && mov.EmpleadoNuevoId.HasValue;
            bool cambioZona = mov.ZonaAnteriorId != mov.ZonaNuevaId && mov.ZonaNuevaId.HasValue;

            if (cambioEmpleado && cambioZona) return "Reasignación y reubicación";
            if (cambioEmpleado) return "Reasignación";
            if (cambioZona) return "Reubicación";
            return "Actualización";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}