using System;

namespace InventarioComputo.Domain.DTOs
{
    public class ReporteEquipoDTO
    {
        public int Id { get; set; }
        public string NumeroSerie { get; set; } = string.Empty;
        public string EtiquetaInventario { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string TipoEquipo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Sede { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Zona { get; set; } = string.Empty;
        public string UsuarioAsignado { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public DateTime FechaAdquisicion { get; set; }
        public decimal Costo { get; set; }
        public bool Activo { get; set; }
    }
}