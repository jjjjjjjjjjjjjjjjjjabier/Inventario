using System.Collections.Generic;

namespace InventarioComputo.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string PasswordHash { get; set; }
        public bool Activo { get; set; }

        public virtual ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
        public virtual ICollection<EquipoComputo> EquiposAsignados { get; set; } = new List<EquipoComputo>();
    }
}