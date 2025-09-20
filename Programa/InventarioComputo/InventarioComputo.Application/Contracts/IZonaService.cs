using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Contracts
{
    public interface IZonaService
    {
        Task<IReadOnlyList<Zona>> BuscarAsync(int areaId, string? filtro, CancellationToken ct);
        Task<Zona> GuardarAsync(Zona entidad, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);
    }
}
