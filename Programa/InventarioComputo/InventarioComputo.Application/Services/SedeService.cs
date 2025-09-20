using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class SedeService : ISedeService
    {
        private readonly ISedeRepository _repo;

        public SedeService(ISedeRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Sede>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default)
        {
            var data = await _repo.BuscarAsync(filtro, incluirInactivas, ct);
            return data.ToList();
        }

        public Task<Sede?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return _repo.ObtenerPorIdAsync(id, ct);
        }

        public async Task<Sede> GuardarAsync(Sede entidad, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre))
            {
                throw new ArgumentException("El nombre de la sede es obligatorio.");
            }

            int? idExcluir = entidad.Id == 0 ? null : entidad.Id;
            if (await _repo.ExisteNombreAsync(entidad.Nombre, idExcluir, ct))
            {
                throw new InvalidOperationException($"Ya existe una sede con el nombre '{entidad.Nombre}'.");
            }

            return await _repo.GuardarAsync(entidad, ct);
        }

        public Task EliminarAsync(int id, CancellationToken ct = default)
        {
            return _repo.EliminarAsync(id, ct);
        }
    }
}