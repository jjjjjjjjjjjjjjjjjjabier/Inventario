using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Infrastructure.Extensions
{
    public static class EntityExtensions
    {
        public static T ClearNavigationProperties<T>(this T entity) where T : class
        {
            // Desacoplar navegaciones según tipo de entidad
            if (entity is EquipoComputo equipo)
            {
                equipo.TipoEquipo = null;
                equipo.Estado = null;
                equipo.Zona = null;
                equipo.Empleado = null;
                equipo.Area = null;
                equipo.Sede = null;
                equipo.HistorialMovimientos = null;
            }
            else if (entity is HistorialMovimiento historial)
            {
                historial.EquipoComputo = null;
                historial.EmpleadoAnterior = null;
                historial.EmpleadoNuevo = null;
                historial.ZonaAnterior = null;
                historial.ZonaNueva = null;
                historial.AreaAnterior = null;
                historial.AreaNueva = null;
                historial.SedeAnterior = null;
                historial.SedeNueva = null;
            }
            else if (entity is Zona zona)
            {
                zona.Area = null;
            }
            else if (entity is Area area)
            {
                area.Sede = null;
                area.Zonas = null;
            }
            
            return entity;
        }
    }
}