using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IMovimientoService
    {
        Task<HistorialMovimiento> RegistrarMovimientoAsync(
            int equipoId,
            int? nuevoUsuarioId,
            int? nuevaZonaId,
            string motivo,
            int usuarioRegistraId,
            CancellationToken ct = default);

        Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialPorEquipoAsync(
            int equipoId,
            CancellationToken ct = default);
    }
}