using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IUnidadRepository
    {
        Task<IReadOnlyList<Unidad>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default);
        Task<Unidad?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Unidad> GuardarAsync(Unidad entidad, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
        Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null, CancellationToken ct = default);
        Task<bool> ExisteAbreviaturaAsync(string abreviatura, int? idExcluir = null, CancellationToken ct = default);
    }
}