using System.Collections.Generic;

namespace InventarioComputo.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        
        // Nuevo campo para distinguir entre usuarios del sistema y empleados
        public bool EsEmpleadoSolamente { get; set; }
        
        public virtual ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
        public virtual ICollection<EquipoComputo> EquiposAsignados { get; set; } = new List<EquipoComputo>();
    }
}