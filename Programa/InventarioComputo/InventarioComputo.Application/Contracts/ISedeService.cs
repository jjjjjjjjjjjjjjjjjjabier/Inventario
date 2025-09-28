using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface ISedeService
    {
        Task<IReadOnlyList<Sede>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default);
        Task<Sede?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Sede> GuardarAsync(Sede entidad, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}