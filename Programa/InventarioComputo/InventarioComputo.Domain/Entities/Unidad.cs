using System;

namespace InventarioComputo.Domain.Entities
{
    public class Unidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Abreviatura { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}
