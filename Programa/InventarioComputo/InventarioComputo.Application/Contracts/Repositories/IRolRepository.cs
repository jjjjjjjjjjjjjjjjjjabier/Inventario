using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IRolRepository
    {
        Task<IReadOnlyList<Rol>> ObtenerTodosAsync(CancellationToken ct = default);
        Task<Rol?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default);
        Task<IReadOnlyList<Rol>> ObtenerRolesPorUsuarioIdAsync(int usuarioId, CancellationToken ct = default);
        Task<Rol> GuardarAsync(Rol rol, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
        Task<int> CrearRolAsync(Rol rol, CancellationToken ct = default);
        Task AsignarRolAUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default);
    }
}