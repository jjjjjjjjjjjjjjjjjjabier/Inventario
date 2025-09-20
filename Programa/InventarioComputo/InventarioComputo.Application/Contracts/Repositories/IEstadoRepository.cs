using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IEstadoRepository
    {
        Task<IReadOnlyList<Estado>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct);
        Task<Estado?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken ct);
        Task<Estado> GuardarAsync(Estado entidad, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);
    }
}