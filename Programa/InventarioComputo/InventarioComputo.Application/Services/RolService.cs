using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _repo;

        public RolService(IRolRepository repo)
        {
            _repo = repo;
        }

        // Implementación correcta con el parámetro incluirInactivos
        public Task<IReadOnlyList<Rol>> ObtenerTodosAsync(bool incluirInactivos = false, CancellationToken ct = default)
            => _repo.ObtenerTodosAsync(ct);

        public async Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken ct = default)
        {
            // Intenta nombre tal cual y con normalización (singular/plural)
            var rol = await _repo.ObtenerPorNombreAsync(nombre, ct);
            if (rol != null) return rol;

            var normalizado = NormalizarRol(nombre);
            if (!string.Equals(normalizado, nombre, StringComparison.OrdinalIgnoreCase))
            {
                rol = await _repo.ObtenerPorNombreAsync(normalizado, ct);
            }
            return rol;
        }

        public Task<Rol?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => _repo.ObtenerPorIdAsync(id, ct);

        public Task<Rol> GuardarAsync(Rol rol, CancellationToken ct = default)
            => _repo.GuardarAsync(rol, ct);

        public Task EliminarAsync(int id, CancellationToken ct = default)
            => _repo.EliminarAsync(id, ct);

        // Métodos auxiliares para trabajar con roles de usuario
        public Task<IReadOnlyList<Rol>> ObtenerRolesPorUsuarioIdAsync(int usuarioId, CancellationToken ct = default)
            => _repo.ObtenerRolesPorUsuarioIdAsync(usuarioId, ct);

        public Task AsignarRolAUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
            => _repo.AsignarRolAUsuarioAsync(usuarioId, rolId, ct);

        public Task QuitarRolAUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
            => _repo.QuitarRolAUsuarioAsync(usuarioId, rolId, ct);

        private static string NormalizarRol(string nombre) => nombre switch
        {
            "Administrador" => "Administradores",
            "Admin" => "Administradores",
            "Consultas" => "Consulta",
            _ => nombre
        };
    }
}