using System;

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
        public int TipoEquipoId { get; set; }
        public virtual TipoEquipo TipoEquipo { get; set; }
        public int EstadoId { get; set; }
        public virtual Estado Estado { get; set; }
        public int ZonaId { get; set; }
        public virtual Zona Zona { get; set; }
        public int? UsuarioId { get; set; }
    }
}