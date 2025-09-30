namespace InventarioComputo.Domain.Entities
{
    public class TipoEquipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Inicializar a string.Empty
        public bool Activo { get; set; }
    }
}