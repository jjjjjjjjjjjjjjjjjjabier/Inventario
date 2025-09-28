using System;

namespace InventarioComputo.Domain.DTOs
{
    public class FiltroReporteDTO
    {
        public int? SedeId { get; set; }
        public int? AreaId { get; set; }
        public int? ZonaId { get; set; }
        public int? EstadoId { get; set; }
        public int? TipoEquipoId { get; set; }
        public int? UsuarioId { get; set; }
        public bool IncluirInactivos { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}