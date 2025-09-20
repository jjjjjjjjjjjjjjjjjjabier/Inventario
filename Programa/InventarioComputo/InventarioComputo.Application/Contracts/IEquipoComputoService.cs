using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IEquipoComputoService
    {
        Task<IReadOnlyList<EquipoComputo>> ObtenerTodosAsync(bool incluirInactivos, CancellationToken ct = default);
        Task<EquipoComputo?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<EquipoComputo> AgregarAsync(EquipoComputo equipo, CancellationToken ct = default);
        Task ActualizarAsync(EquipoComputo equipo, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}