using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class UnidadService : IUnidadService
    {
        private readonly IUnidadRepository _repo;

        public UnidadService(IUnidadRepository repo)
        {
            _repo = repo;
        }

        public async Task<IReadOnlyList<Unidad>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default)
        {
            return await _repo.BuscarAsync(filtro, incluirInactivas, ct);
        }

        public Task<Unidad?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return _repo.ObtenerPorIdAsync(id, ct);
        }

        public Task<Unidad> GuardarAsync(Unidad entidad, CancellationToken ct = default)
        {
            return _repo.GuardarAsync(entidad, ct);
        }

        public Task EliminarAsync(int id, CancellationToken ct = default)
        {
            return _repo.EliminarAsync(id, ct);
        }

        public Task<bool> ExisteNombreAsync(string nombre, int? idExcluir, CancellationToken ct = default)
        {
            return _repo.ExisteNombreAsync(nombre, idExcluir, ct);
        }

        public Task<bool> ExisteAbreviaturaAsync(string abreviatura, int? idExcluir, CancellationToken ct = default)
        {
            return _repo.ExisteAbreviaturaAsync(abreviatura, idExcluir, ct);
        }
    }
}