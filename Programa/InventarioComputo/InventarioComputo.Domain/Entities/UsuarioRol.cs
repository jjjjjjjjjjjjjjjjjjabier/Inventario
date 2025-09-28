using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioComputo.Domain.Entities
{
    public class UsuarioRol
    {
        public int UsuarioId { get; set; }
        
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
        
        public int RolId { get; set; }
        
        [ForeignKey("RolId")]
        public virtual Rol Rol { get; set; }
    }
}