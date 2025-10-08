using System;

namespace InventarioComputo.Domain.Entities
{
    public class BitacoraEvento
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Entidad { get; set; } = string.Empty;  
        public string Accion { get; set; } = string.Empty;   
        public int EntidadId { get; set; }
        public int? UsuarioResponsableId { get; set; }
        public string? Detalles { get; set; }
    }
}