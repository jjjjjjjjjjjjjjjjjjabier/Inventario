using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioComputo.Domain.Entities
{
    public class HistorialMovimiento
    {
        public int Id { get; set; }
        
        public int EquipoComputoId { get; set; }
        [ForeignKey("EquipoComputoId")]
        public virtual EquipoComputo EquipoComputo { get; set; }
        
        public int? UsuarioAnteriorId { get; set; }
        [ForeignKey("UsuarioAnteriorId")]
        public virtual Usuario? UsuarioAnterior { get; set; }
        
        public int? UsuarioNuevoId { get; set; }
        [ForeignKey("UsuarioNuevoId")]
        public virtual Usuario? UsuarioNuevo { get; set; }
        
        // Nuevos campos para auditoría de ubicación
        public int? ZonaAnteriorId { get; set; }
        [ForeignKey("ZonaAnteriorId")]
        public virtual Zona? ZonaAnterior { get; set; }
        
        public int? ZonaNuevaId { get; set; }
        [ForeignKey("ZonaNuevaId")]
        public virtual Zona? ZonaNueva { get; set; }
        
        public DateTime FechaMovimiento { get; set; }
        public string Motivo { get; set; }
        
        public int UsuarioResponsableId { get; set; }
        [ForeignKey("UsuarioResponsableId")]
        public virtual Usuario UsuarioResponsable { get; set; }
    }
}