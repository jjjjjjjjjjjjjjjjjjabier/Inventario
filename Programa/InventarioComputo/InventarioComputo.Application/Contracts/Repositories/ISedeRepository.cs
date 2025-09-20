using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface ISedeRepository
    {
        Task<IReadOnlyList<Sede>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct);
        Task<Sede?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken ct);
        Task<Sede> GuardarAsync(Sede entidad, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);
    }
}