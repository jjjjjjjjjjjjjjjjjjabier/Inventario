using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Contracts
{
    public interface IHistorialMovimientoService
    {
        Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialEquipoAsync(int equipoId, CancellationToken ct = default);
        Task RegistrarMovimientoAsync(int equipoId, int? usuarioNuevoId, int? zonaNuevaId, 
            string motivo, int usuarioResponsableId, CancellationToken ct = default);
    }
}