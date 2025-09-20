using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioComputo.Domain.Entities
{
    public class EquipoComputo
    {
        public int Id { get; set; }
        public string NumeroSerie { get; set; }
        public string EtiquetaInventario { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Caracteristicas { get; set; }
        public string? Observaciones { get; set; }
        public DateTime? FechaAdquisicion { get; set; }

        // **NUEVA PROPIEDAD**
        public bool Activo { get; set; } = true;

        public int TipoEquipoId { get; set; }
        [ForeignKey("TipoEquipoId")]
        public virtual TipoEquipo TipoEquipo { get; set; }

        public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public virtual Estado Estado { get; set; }

        public int? ZonaId { get; set; }
        [ForeignKey("ZonaId")]
        public virtual Zona? Zona { get; set; }

        public int? UsuarioId { get; set; }
    }
}