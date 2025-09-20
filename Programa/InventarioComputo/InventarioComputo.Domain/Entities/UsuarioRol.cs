namespace InventarioComputo.Domain.Entities
{
    public class UsuarioRol
    {
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public int RolId { get; set; }
        public virtual Rol Rol { get; set; }
    }
}