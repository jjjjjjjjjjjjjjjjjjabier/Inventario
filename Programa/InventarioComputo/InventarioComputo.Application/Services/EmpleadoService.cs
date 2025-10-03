using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _repo;

        public EmpleadoService(IEmpleadoRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyList<Empleado>> BuscarAsync(string? filtro, bool incluirInactivos, CancellationToken ct = default)
            => _repo.BuscarAsync(filtro, incluirInactivos, ct);

        public Task<Empleado?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => _repo.ObtenerPorIdAsync(id, ct);

        public async Task<Empleado> GuardarAsync(Empleado entidad, CancellationToken ct = default)
        {
            if (entidad is null) throw new ArgumentNullException(nameof(entidad));

            entidad.NombreCompleto = entidad.NombreCompleto?.Trim() ?? string.Empty;
            entidad.Puesto = entidad.Puesto?.Trim();

            if (string.IsNullOrWhiteSpace(entidad.NombreCompleto))
                throw new ArgumentException("El nombre completo del empleado es obligatorio.", nameof(entidad.NombreCompleto));

            if (entidad.NombreCompleto.Length > 200)
                throw new ArgumentException("El nombre completo no debe exceder 200 caracteres.", nameof(entidad.NombreCompleto));

            if (entidad.Puesto?.Length > 100)
                throw new ArgumentException("El puesto no debe exceder 100 caracteres.", nameof(entidad.Puesto));

            return await _repo.GuardarAsync(entidad, ct);
        }

        public Task EliminarAsync(int id, CancellationToken ct = default)
            => _repo.EliminarAsync(id, ct);
    }
}