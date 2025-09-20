using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IZonaRepository
    {
        Task<IReadOnlyList<Zona>> BuscarAsync(int areaId, string? filtro, CancellationToken ct);
        Task<Zona?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<bool> ExisteNombreAsync(int areaId, string nombre, int? excluirId, CancellationToken ct);
        Task<Zona> GuardarAsync(Zona entidad, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);
    }
}