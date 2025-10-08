using InventarioComputo.Domain.Entities;
using System;

namespace InventarioComputo.UI.ViewModels
{
    public class MovimientoItemViewModel
    {
        private readonly HistorialMovimiento _m;

        public MovimientoItemViewModel(HistorialMovimiento m) => _m = m;

        public DateTime FechaMovimiento => _m.FechaMovimiento;
        public string Motivo => _m.Motivo;

        // Nombres seguros para evitar bindings anidados con nulls
        public string ResponsableNombre => _m.UsuarioResponsable?.NombreCompleto ?? "-";
        public string EmpleadoAnteriorNombre => _m.EmpleadoAnterior?.NombreCompleto ?? "Sin asignación";
        public string EmpleadoNuevoNombre => _m.EmpleadoNuevo?.NombreCompleto ?? "Sin asignación";

        // Textos de ubicación renderizados
        public string UbicacionAnteriorTexto =>
            _m.ZonaAnterior != null
                ? $"{_m.ZonaAnterior.Area?.Sede?.Nombre ?? "-"} / {_m.ZonaAnterior.Area?.Nombre ?? "-"} / {_m.ZonaAnterior.Nombre}"
                : _m.AreaAnterior != null || _m.SedeAnterior != null
                    ? $"{_m.SedeAnterior?.Nombre ?? "-"} / {_m.AreaAnterior?.Nombre ?? "-"} / -"
                    : "Sin ubicación";

        public string UbicacionNuevaTexto =>
            _m.ZonaNueva != null
                ? $"{_m.ZonaNueva.Area?.Sede?.Nombre ?? "-"} / {_m.ZonaNueva.Area?.Nombre ?? "-"} / {_m.ZonaNueva.Nombre}"
                : _m.AreaNueva != null || _m.SedeNueva != null
                    ? $"{_m.SedeNueva?.Nombre ?? "-"} / {_m.AreaNueva?.Nombre ?? "-"} / -"
                    : "Sin ubicación";
    }
}