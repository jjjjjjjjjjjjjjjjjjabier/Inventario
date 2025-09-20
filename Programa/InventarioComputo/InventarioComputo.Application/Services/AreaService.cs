using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Services
{
    public sealed class AreaService : IAreaService
    {
        private readonly IAreaRepository _repo;
        public AreaService(IAreaRepository repo) => _repo = repo;

        public Task<IReadOnlyList<Area>> BuscarAsync(int sedeId, string? filtro, CancellationToken ct = default)
            => _repo.BuscarAsync(sedeId, filtro, ct);

        public async Task<Area> GuardarAsync(Area entidad, CancellationToken ct = default)
        {
            if (entidad.SedeId <= 0) throw new ArgumentException("Sede obligatoria.");
            if (string.IsNullOrWhiteSpace(entidad.Nombre)) throw new ArgumentException("Nombre obligatorio.");

            int? idExcluir = entidad.Id == 0 ? null : entidad.Id;
            if (await _repo.ExisteNombreAsync(entidad.SedeId, entidad.Nombre, idExcluir, ct))
                throw new InvalidOperationException("Ya existe un área con ese nombre en la sede.");

            return await _repo.GuardarAsync(entidad, ct);
        }

        public Task EliminarAsync(int id, CancellationToken ct = default) => _repo.EliminarAsync(id, ct);
    }
}