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
        private readonly IPasswordHasher _hasher;
        private readonly IRolRepository _rolRepo;

        public UsuarioService(IUsuarioRepository repo, IPasswordHasher hasher, IRolRepository rolRepo)
        {
            _repo = repo;
            _hasher = hasher;
            _rolRepo = rolRepo;
        }

        public async Task<bool> AuthenticateAsync(string nombreUsuario, string password, CancellationToken ct = default)
        {
            var usuario = await _repo.ObtenerPorNombreUsuarioAsync(nombreUsuario, ct);
            if (usuario == null || !usuario.Activo) return false;
            return _hasher.VerifyPassword(password, usuario.PasswordHash);
        }

        public Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default)
            => _repo.ObtenerPorNombreUsuarioAsync(nombreUsuario, ct);

        public Task<IReadOnlyList<Rol>> ObtenerRolesDeUsuarioAsync(int usuarioId, CancellationToken ct = default)
            => _rolRepo.ObtenerRolesPorUsuarioIdAsync(usuarioId, ct);

        public Task<IReadOnlyList<Usuario>> BuscarAsync(string? filtro, bool incluirInactivos = false, CancellationToken ct = default)
            => _repo.BuscarAsync(filtro, incluirInactivos, ct);

        public Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
            => _repo.ObtenerPorIdAsync(id, ct);

        public async Task<Usuario> GuardarAsync(Usuario usuario, string? password = null, CancellationToken ct = default)
        {
            if (usuario is null) throw new ArgumentNullException(nameof(usuario));
            usuario.NombreUsuario = usuario.NombreUsuario?.Trim() ?? string.Empty;
            usuario.NombreCompleto = usuario.NombreCompleto?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario)) throw new ArgumentException("El nombre de usuario es obligatorio.", nameof(usuario.NombreUsuario));
            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto)) throw new ArgumentException("El nombre completo es obligatorio.", nameof(usuario.NombreCompleto));

            if (await _repo.ExisteNombreUsuarioAsync(usuario.NombreUsuario, usuario.Id == 0 ? null : usuario.Id, ct))
                throw new InvalidOperationException($"Ya existe un usuario con el nombre '{usuario.NombreUsuario}'.");

            if (!string.IsNullOrEmpty(password))
            {
                usuario.PasswordHash = _hasher.HashPassword(password);
            }

            return await _repo.GuardarAsync(usuario, ct);
        }

        public Task AsignarRolUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
            => _rolRepo.AsignarRolAUsuarioAsync(usuarioId, rolId, ct);

        public Task QuitarRolUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default)
            => _rolRepo.QuitarRolAUsuarioAsync(usuarioId, rolId, ct);

        public Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idExcluir = null, CancellationToken ct = default)
            => _repo.ExisteNombreUsuarioAsync(nombreUsuario, idExcluir, ct);

        public Task EliminarAsync(int id, CancellationToken ct = default)
            => _repo.EliminarAsync(id, ct);
    }
}