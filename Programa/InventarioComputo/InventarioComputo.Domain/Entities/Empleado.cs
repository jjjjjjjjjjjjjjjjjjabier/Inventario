namespace InventarioComputo.Domain.Entities
{
    public class Empleado
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Puesto { get; set; }
        public bool Activo { get; set; } = true;
    }
}