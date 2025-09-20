using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IAreaRepository
    {
        Task<IReadOnlyList<Area>> BuscarAsync(int sedeId, string? filtro, CancellationToken ct);
        Task<Area?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<bool> ExisteNombreAsync(int sedeId, string nombre, int? excluirId, CancellationToken ct);
        Task<Area> GuardarAsync(Area entidad, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);
    }
}