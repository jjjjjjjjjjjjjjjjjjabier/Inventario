namespace InventarioComputo.Domain.Entities
{
    public class Area
    {
        public int Id { get; set; }
        public int SedeId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public Sede? Sede { get; set; }
        public ICollection<Zona> Zonas { get; set; } = new List<Zona>();
    }
}
