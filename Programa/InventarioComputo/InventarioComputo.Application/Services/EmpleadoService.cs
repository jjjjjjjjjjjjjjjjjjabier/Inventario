using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public sealed class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _repo;

        public EmpleadoService(IEmpleadoRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyList<Empleado>> BuscarAsync(string? filtro, bool incluirInactivos = false, CancellationToken ct = default)
            => _repo.BuscarAsync(filtro, incluirInactivos, ct);

        public Task<IReadOnlyList<Empleado>> ObtenerTodosAsync(bool incluirInactivos = false, CancellationToken ct = default)
            => _repo.ObtenerTodosAsync(incluirInactivos, ct);

        public Task<Empleado?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => _repo.ObtenerPorIdAsync(id, ct);

        public async Task<Empleado> GuardarAsync(Empleado entidad, CancellationToken ct = default)
        {
            if (entidad is null) throw new ArgumentNullException(nameof(entidad));
            entidad.NombreCompleto = entidad.NombreCompleto?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(entidad.NombreCompleto))
                throw new ArgumentException("El nombre del empleado es obligatorio.", nameof(entidad.NombreCompleto));
            if (entidad.NombreCompleto.Length > 200)
                throw new ArgumentException("El nombre del empleado no debe exceder 200 caracteres.", nameof(entidad.NombreCompleto));
            if (entidad.Correo?.Length > 150)
                throw new ArgumentException("El correo no debe exceder 150 caracteres.", nameof(entidad.Correo));
            if (entidad.Telefono?.Length > 50)
                throw new ArgumentException("El teléfono no debe exceder 50 caracteres.", nameof(entidad.Telefono));

            int? excluirId = entidad.Id == 0 ? null : entidad.Id;
            if (await _repo.ExisteNombreAsync(entidad.NombreCompleto, excluirId, ct))
                throw new InvalidOperationException($"Ya existe un empleado con el nombre '{entidad.NombreCompleto}'.");

            return await _repo.GuardarAsync(entidad, ct);
        }

        public Task EliminarAsync(int id, CancellationToken ct = default)
            => _repo.EliminarAsync(id, ct);
    }
}