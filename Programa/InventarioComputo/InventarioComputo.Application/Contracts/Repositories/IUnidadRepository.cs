using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IUnidadRepository
    {
        Task<IReadOnlyList<Unidad>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct);
        Task<Unidad?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken ct);
        Task<bool> ExisteAbreviaturaAsync(string abreviatura, int? excluirId, CancellationToken ct);
        Task<Unidad> GuardarAsync(Unidad entidad, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);
    }
}