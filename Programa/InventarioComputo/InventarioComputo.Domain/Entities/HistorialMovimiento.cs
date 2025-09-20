using System;

namespace InventarioComputo.Domain.Entities
{
    public class HistorialMovimiento
    {
        public int Id { get; set; }
        public int EquipoComputoId { get; set; }
        public virtual EquipoComputo EquipoComputo { get; set; }
        public int? UsuarioAnteriorId { get; set; }
        public virtual Usuario? UsuarioAnterior { get; set; }
        public int? UsuarioNuevoId { get; set; }
        public virtual Usuario? UsuarioNuevo { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Motivo { get; set; }
        public int UsuarioResponsableId { get; set; }
        public virtual Usuario UsuarioResponsable { get; set; }
    }
}