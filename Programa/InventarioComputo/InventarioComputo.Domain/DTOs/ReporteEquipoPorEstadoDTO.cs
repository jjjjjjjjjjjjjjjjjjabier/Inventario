namespace InventarioComputo.Domain.DTOs
{
    public class ReporteEquipoPorEstadoDTO
    {
        public string Estado { get; set; } = string.Empty;
        public string ColorHex { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}