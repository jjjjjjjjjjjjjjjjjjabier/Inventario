namespace InventarioComputo.Domain.Entities
{
    public class Estado
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Inicializar a string.Empty
        public string? Descripcion { get; set; }
        public string? ColorHex { get; set; }
        public bool Activo { get; set; }
    }
}