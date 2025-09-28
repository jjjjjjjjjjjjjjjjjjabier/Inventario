using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IAreaRepository
    {
        Task<IReadOnlyList<Area>> BuscarAsync(int sedeId, string? filtro, bool incluirInactivas = false, CancellationToken ct = default);
        Task<bool> ExisteNombreAsync(int sedeId, string nombre, int? excluirId = null, CancellationToken ct = default);
        Task<Area?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Area> GuardarAsync(Area entidad, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}