using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _repo;
        public RolService(IRolRepository repo) => _repo = repo;

        public Task<IReadOnlyList<Rol>> ObtenerTodosAsync(CancellationToken ct = default)
            => _repo.ObtenerTodosAsync(ct);

        public Task<Rol?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => _repo.ObtenerPorIdAsync(id, ct);

        public Task<Rol> GuardarAsync(Rol rol, CancellationToken ct = default)
            => _repo.GuardarAsync(rol, ct);

        public Task EliminarAsync(int id, CancellationToken ct = default)
            => _repo.EliminarAsync(id, ct);
    }
}