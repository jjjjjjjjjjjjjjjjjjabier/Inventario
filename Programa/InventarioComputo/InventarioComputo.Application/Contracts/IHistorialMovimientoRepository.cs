using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Contracts.Legacy
{
    // OBSOLETO: Conservado temporalmente para referencia. No usar en DI ni en servicios.
    public interface ILegacyHistorialMovimientoRepository
    {
        Task<HistorialMovimiento> AgregarAsync(HistorialMovimiento historial, CancellationToken ct = default);
        Task<IReadOnlyList<HistorialMovimiento>> ObtenerPorEquipoAsync(int equipoId, CancellationToken ct = default);
        Task<IReadOnlyList<HistorialMovimiento>> ObtenerPorUsuarioAsync(int usuarioId, CancellationToken ct = default);
    }
}