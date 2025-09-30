using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public sealed class TipoEquipoService : ITipoEquipoService
    {
        private readonly ITipoEquipoRepository _repo;

        public TipoEquipoService(ITipoEquipoRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyList<TipoEquipo>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default)
            => _repo.BuscarAsync(filtro, incluirInactivas, ct);

        public Task<TipoEquipo?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => _repo.ObtenerPorIdAsync(id, ct);

        public async Task<TipoEquipo> GuardarAsync(TipoEquipo entidad, CancellationToken ct = default)
        {
            if (entidad is null) throw new ArgumentNullException(nameof(entidad));

            entidad.Nombre = entidad.Nombre?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(entidad.Nombre))
                throw new ArgumentException("El nombre del tipo de equipo es obligatorio.", nameof(entidad.Nombre));

            if (entidad.Nombre.Length > 100)
                throw new ArgumentException("El nombre del tipo de equipo no debe exceder 100 caracteres.", nameof(entidad.Nombre));

            int? idExcluir = entidad.Id == 0 ? null : entidad.Id;
            if (await _repo.ExisteNombreAsync(entidad.Nombre, idExcluir, ct))
                throw new InvalidOperationException($"Ya existe un tipo de equipo con el nombre '{entidad.Nombre}'.");

            return await _repo.GuardarAsync(entidad, ct);
        }

        public Task EliminarAsync(int id, CancellationToken ct = default)
            => _repo.EliminarAsync(id, ct);
    }
}