namespace InventarioComputo.Domain.Entities
{
    public class Sede
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public ICollection<Area> Areas { get; set; } = new List<Area>();
    }
}
