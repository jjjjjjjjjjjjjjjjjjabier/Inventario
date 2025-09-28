using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IHistorialMovimientoRepository
    {
        Task<HistorialMovimiento> AgregarAsync(HistorialMovimiento entidad, CancellationToken ct = default);
        Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialEquipoAsync(int equipoId, CancellationToken ct);
        Task<IReadOnlyList<HistorialMovimiento>> ObtenerPorEquipoAsync(int equipoId, CancellationToken ct = default);
        Task<IReadOnlyList<HistorialMovimiento>> ObtenerPorUsuarioAsync(int usuarioId, CancellationToken ct = default);
    }
}