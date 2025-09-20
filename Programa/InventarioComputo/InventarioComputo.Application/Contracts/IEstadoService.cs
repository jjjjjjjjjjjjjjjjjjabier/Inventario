using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IEstadoService
    {
        Task<List<Estado>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default);
        Task<Estado?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Estado> GuardarAsync(Estado entidad, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}