using InventarioComputo.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InventarioComputo.Application.Contracts
{
    public interface IAreaService
    {
        Task<IReadOnlyList<Area>> BuscarAsync(int sedeId, string? filtro, CancellationToken ct);
        Task<Area> GuardarAsync(Area entidad, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);
    }
}
