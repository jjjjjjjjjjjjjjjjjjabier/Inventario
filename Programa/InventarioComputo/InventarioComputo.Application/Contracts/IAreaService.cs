using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IAreaService
    {
        Task<IReadOnlyList<Area>> BuscarAsync(int sedeId, string? filtro, bool incluirInactivas = false, CancellationToken ct = default);
        Task<Area?> ObtenerPorIdAsync(int id, CancellationToken ct = default); // Método que faltaba
        Task<Area> GuardarAsync(Area entidad, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}
