using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IZonaRepository
    {
        Task<IReadOnlyList<Zona>> BuscarAsync(int areaId, string? filtro, bool incluirInactivas = false, CancellationToken ct = default);
        Task<IReadOnlyList<Zona>> ObtenerTodasAsync(bool incluirInactivas = false, CancellationToken ct = default);
        Task<bool> ExisteNombreAsync(int areaId, string nombre, int? excluirId = null, CancellationToken ct = default);
        Task<Zona?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Zona> GuardarAsync(Zona entidad, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}