using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IMovimientoService
    {
        Task<IReadOnlyList<HistorialMovimiento>> ObtenerHistorialPorEquipoAsync(int equipoId, CancellationToken ct = default);
        Task AsignarEquipoAsync(int equipoId, int? empleadoId, int? zonaId, string motivo, CancellationToken ct = default);
        Task RegistrarMovimientoAsync(int equipoId, int? empleadoNuevoId, int? zonaNuevaId, string motivo, int usuarioResponsableId, CancellationToken ct = default);
    }
}