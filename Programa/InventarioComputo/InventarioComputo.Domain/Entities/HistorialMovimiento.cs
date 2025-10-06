using System;

namespace InventarioComputo.Domain.Entities
{
    public class HistorialMovimiento
    {
        public int Id { get; set; }
        public int EquipoComputoId { get; set; }
        public DateTime FechaMovimiento { get; set; }

        public virtual EquipoComputo? EquipoComputo { get; set; }

        public int? EmpleadoAnteriorId { get; set; }
        public virtual Empleado? EmpleadoAnterior { get; set; }

        public int? SedeAnteriorId { get; set; }
        public virtual Sede? SedeAnterior { get; set; }

        public int? AreaAnteriorId { get; set; }
        public virtual Area? AreaAnterior { get; set; }

        public int? ZonaAnteriorId { get; set; }
        public virtual Zona? ZonaAnterior { get; set; }

        public int? EmpleadoNuevoId { get; set; }
        public virtual Empleado? EmpleadoNuevo { get; set; }

        public int? SedeNuevaId { get; set; }
        public virtual Sede? SedeNueva { get; set; }

        public int? AreaNuevaId { get; set; }
        public virtual Area? AreaNueva { get; set; }

        public int? ZonaNuevaId { get; set; }
        public virtual Zona? ZonaNueva { get; set; }

        public string Motivo { get; set; } = string.Empty;

        public int UsuarioResponsableId { get; set; }
        // Requerido por FK, pero sin inicializador
        public virtual Usuario? UsuarioResponsable { get; set; }
    }
}