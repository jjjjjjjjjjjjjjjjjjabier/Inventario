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
    public class EstadoService : IEstadoService
    {
        private readonly IEstadoRepository _repo;

        public EstadoService(IEstadoRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Estado>> BuscarAsync(string? filtro, bool incluirInactivas, CancellationToken ct = default)
        {
            var resultado = await _repo.BuscarAsync(filtro, incluirInactivas, ct);
            return resultado.ToList();
        }

        public Task<Estado?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return _repo.ObtenerPorIdAsync(id, ct);
        }

        public async Task<Estado> GuardarAsync(Estado entidad, CancellationToken ct = default)
        {
            if (entidad == null)
                throw new ArgumentNullException(nameof(entidad));

            if (string.IsNullOrWhiteSpace(entidad.Nombre))
                throw new ArgumentException("El nombre del estado no puede estar vacío.", nameof(entidad.Nombre));

            if (await _repo.ExisteNombreAsync(entidad.Nombre, entidad.Id, ct))
            {
                throw new InvalidOperationException($"Ya existe un estado con el nombre '{entidad.Nombre}'.");
            }

            return await _repo.GuardarAsync(entidad, ct);
        }

        public Task EliminarAsync(int id, CancellationToken ct = default)
        {
            return _repo.EliminarAsync(id, ct);
        }
    }
}