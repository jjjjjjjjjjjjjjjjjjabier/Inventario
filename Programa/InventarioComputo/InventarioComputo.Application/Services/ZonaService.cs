using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;

namespace InventarioComputo.Application.Services
{
    public sealed class ZonaService : IZonaService
    {
        private readonly IZonaRepository _repo;
        public ZonaService(IZonaRepository repo) => _repo = repo;

        public Task<IReadOnlyList<Zona>> BuscarAsync(int areaId, string? filtro, CancellationToken ct = default)
            => _repo.BuscarAsync(areaId, filtro, ct);

        public async Task<Zona> GuardarAsync(Zona entidad, CancellationToken ct = default)
        {
            if (entidad.AreaId <= 0) throw new ArgumentException("Área obligatoria.");
            if (string.IsNullOrWhiteSpace(entidad.Nombre)) throw new ArgumentException("Nombre obligatorio.");

            int? idExcluir = entidad.Id == 0 ? null : entidad.Id;
            if (await _repo.ExisteNombreAsync(entidad.AreaId, entidad.Nombre, idExcluir, ct))
                throw new InvalidOperationException("Ya existe una zona con ese nombre en el área.");

            return await _repo.GuardarAsync(entidad, ct);
        }

        public Task EliminarAsync(int id, CancellationToken ct = default) => _repo.EliminarAsync(id, ct);
    }
}