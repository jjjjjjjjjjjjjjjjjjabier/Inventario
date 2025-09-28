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

        public async Task<Unidad> GuardarAsync(Unidad entidad, CancellationToken ct = default)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            entidad.Nombre = entidad.Nombre?.Trim() ?? string.Empty;
            entidad.Abreviatura = entidad.Abreviatura?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(entidad.Nombre))
                throw new ArgumentException("El nombre de la unidad es obligatorio.", nameof(entidad.Nombre));

            if (string.IsNullOrWhiteSpace(entidad.Abreviatura))
                throw new ArgumentException("La abreviatura de la unidad es obligatoria.", nameof(entidad.Abreviatura));

            // Validaciones de unicidad coherentes con índices únicos en DB
            int? idExcluir = entidad.Id == 0 ? null : entidad.Id;

            if (await _repo.ExisteNombreAsync(entidad.Nombre, idExcluir, ct))
                throw new InvalidOperationException($"Ya existe una unidad con el nombre '{entidad.Nombre}'.");

            if (await _repo.ExisteAbreviaturaAsync(entidad.Abreviatura, idExcluir, ct))
                throw new InvalidOperationException($"Ya existe una unidad con la abreviatura '{entidad.Abreviatura}'.");

            return await _repo.GuardarAsync(entidad, ct);
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