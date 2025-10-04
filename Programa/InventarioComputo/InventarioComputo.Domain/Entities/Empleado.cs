using System.Collections.Generic;

namespace InventarioComputo.Domain.Entities
{
    public class Empleado
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public bool Activo { get; set; } = true;

        public virtual ICollection<EquipoComputo> EquiposAsignados { get; set; } = new List<EquipoComputo>();
    }
}