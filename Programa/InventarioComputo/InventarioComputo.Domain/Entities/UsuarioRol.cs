namespace InventarioComputo.Domain.Entities
{
    public class UsuarioRol
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int RolId { get; set; }

        public virtual Usuario Usuario { get; set; } = new(); // Inicializar
        public virtual Rol Rol { get; set; } = new();       // Inicializar
    }
}