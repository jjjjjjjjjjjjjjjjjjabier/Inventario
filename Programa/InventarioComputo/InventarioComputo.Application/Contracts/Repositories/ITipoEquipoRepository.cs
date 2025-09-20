using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface ITipoEquipoRepository
    {
        Task<IReadOnlyList<TipoEquipo>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct);
        Task<TipoEquipo?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<TipoEquipo> GuardarAsync(TipoEquipo entidad, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);
        Task<bool> ExisteNombreAsync(string nombre, int? idExcluir, CancellationToken ct);
    }
}