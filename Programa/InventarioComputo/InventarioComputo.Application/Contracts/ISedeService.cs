using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Contracts
{
    public interface ISedeService
    {
        Task<List<Sede>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default);
        Task<Sede?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Sede> GuardarAsync(Sede entidad, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}