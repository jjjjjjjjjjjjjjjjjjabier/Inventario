using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IRolService
    {
        Task<IReadOnlyList<Rol>> ObtenerTodosAsync(bool incluirInactivos = false, CancellationToken ct = default);
        Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default);
        Task<Rol?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Rol> GuardarAsync(Rol rol, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}