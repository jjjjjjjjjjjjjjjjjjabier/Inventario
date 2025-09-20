namespace InventarioComputo.Domain.Entities
{
    public class Zona
    {
        public int Id { get; set; }
        public int AreaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public Area? Area { get; set; }
    }
}
