using System;

namespace InventarioComputo.Domain.DTOs
{
    public class ReporteEquipoDTO
    {
        public int Id { get; set; }
        public string NumeroSerie { get; set; }
        public string EtiquetaInventario { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string TipoEquipo { get; set; }
        public string Estado { get; set; }
        public string Usuario { get; set; }
        public string Sede { get; set; }
        public string Area { get; set; }
        public string Zona { get; set; }
        public string UsuarioAsignado { get; set; }
        public string Ubicacion { get; set; }
        public DateTime? FechaAdquisicion { get; set; }
        public bool Activo { get; set; }
    }
}