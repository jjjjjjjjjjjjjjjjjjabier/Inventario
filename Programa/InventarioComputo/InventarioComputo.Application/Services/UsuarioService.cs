using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IPasswordHasher _passwordHasher;

        public UsuarioService(IUsuarioRepository repo, IPasswordHasher passwordHasher)
        {
            _repo = repo;
            _passwordHasher = passwordHasher;
        }

        public Task<bool> AuthenticateAsync(string nombreUsuario, string password, CancellationToken ct = default)
        {
            return _repo.AuthenticateAsync(nombreUsuario, password, ct);
        }

        public Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default)
        {
            return _repo.ObtenerPorNombreUsuarioAsync(nombreUsuario, ct);
        }

        public Task<IReadOnlyList<Usuario>> BuscarAsync(string? filtro, bool incluirInactivos = false, CancellationToken ct = default)
        {
            return _repo.BuscarAsync(filtro, incluirInactivos, ct);
        }

        public Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return _repo.ObtenerPorIdAsync(id, ct);
        }

        public Task<IReadOnlyList<Rol>> ObtenerRolesDeUsuarioAsync(int usuarioId, CancellationToken ct = default)
        {
            return _repo.ObtenerRolesDeUsuarioAsync(usuarioId, ct);
        }

        public Task AsignarRolUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
        {
            return _repo.AsignarRolUsuarioAsync(usuarioId, rolId, ct);
        }

        public Task QuitarRolUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
        {
            return _repo.QuitarRolUsuarioAsync(usuarioId, rolId, ct);
        }

        // Método que faltaba implementar
        public Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idExcluir = null, CancellationToken ct = default)
        {
            return _repo.ExisteNombreUsuarioAsync(nombreUsuario, idExcluir, ct);
        }

        // Método que faltaba implementar
        public async Task<Usuario> GuardarAsync(Usuario usuario, string? password = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio");

            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto))
                throw new ArgumentException("El nombre completo es obligatorio");

            // Si es usuario nuevo, password es obligatorio
            if (usuario.Id == 0 && string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña es obligatoria para nuevos usuarios");

            // Verificar nombre único
            if (await ExisteNombreUsuarioAsync(usuario.NombreUsuario, usuario.Id, ct))
                throw new InvalidOperationException($"Ya existe un usuario con el nombre '{usuario.NombreUsuario}'");

            // Si hay password, actualizar hash
            if (!string.IsNullOrEmpty(password))
                usuario.PasswordHash = _passwordHasher.HashPassword(password);

            return await _repo.GuardarAsync(usuario, ct);
        }

        // Método que faltaba implementar
        public Task EliminarAsync(int id, CancellationToken ct = default)
        {
            return _repo.EliminarAsync(id, ct);
        }
    }
}