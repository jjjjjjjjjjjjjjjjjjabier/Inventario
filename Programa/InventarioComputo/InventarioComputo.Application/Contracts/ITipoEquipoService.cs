using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface ITipoEquipoService
    {
        Task<IReadOnlyList<TipoEquipo>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default);

        Task<TipoEquipo?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

        Task<TipoEquipo> GuardarAsync(TipoEquipo entidad, CancellationToken ct = default);

        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}