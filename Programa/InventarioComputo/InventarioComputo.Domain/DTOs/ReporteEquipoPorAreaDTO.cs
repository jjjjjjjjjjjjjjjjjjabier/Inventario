namespace InventarioComputo.Domain.DTOs
{
    public class ReporteEquipoPorAreaDTO
    {
        public string Sede { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}