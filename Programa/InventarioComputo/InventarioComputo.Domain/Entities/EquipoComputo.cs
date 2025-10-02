using System;
using System.Collections.Generic;

namespace InventarioComputo.Domain.Entities
{
    public class EquipoComputo
    {
        public int Id { get; set; }
        public string NumeroSerie { get; set; } = string.Empty;
        public string EtiquetaInventario { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Caracteristicas { get; set; } = string.Empty;
        public string? Observaciones { get; set; } // Propiedad faltante
        public DateTime? FechaAdquisicion { get; set; } // Cambiado a nullable
        public decimal Costo { get; set; }
        public bool Activo { get; set; }

        public int TipoEquipoId { get; set; }
        public virtual TipoEquipo TipoEquipo { get; set; } = new();

        public int EstadoId { get; set; }
        public virtual Estado Estado { get; set; } = new();

        // Renombrado a UsuarioId para mantener coherencia con los servicios existentes
        public int? UsuarioId { get; set; }
        public virtual Usuario? Usuario { get; set; }

        public int? SedeId { get; set; }
        public virtual Sede? Sede { get; set; }

        public int? AreaId { get; set; }
        public virtual Area? Area { get; set; }

        public int? ZonaId { get; set; }
        public virtual Zona? Zona { get; set; }

        public virtual ICollection<HistorialMovimiento> HistorialMovimientos { get; set; } = new List<HistorialMovimiento>();
    }
}