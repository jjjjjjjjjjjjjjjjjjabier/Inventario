using System.Collections.Generic;

namespace InventarioComputo.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty; // Inicializar
        public string NombreCompleto { get; set; } = string.Empty; // Inicializar
        public string PasswordHash { get; set; } = string.Empty; // Inicializar
        public bool Activo { get; set; }

        public virtual ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
        public virtual ICollection<EquipoComputo> EquiposAsignados { get; set; } = new List<EquipoComputo>();
    }
}