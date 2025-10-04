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
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IRolRepository _rolRepo;
        private readonly IPasswordHasher _hasher;

        public AuthService(IUsuarioRepository usuarioRepo, IRolRepository rolRepo, IPasswordHasher hasher)
        {
            _usuarioRepo = usuarioRepo;
            _rolRepo = rolRepo;
            _hasher = hasher;
        }

        public async Task<Usuario> AuthenticateAsync(string username, string password, CancellationToken ct = default)
        {
            // Reutiliza el repositorio (hash/validación interna)
            var ok = await _usuarioRepo.VerificarCredencialesAsync(username, password, ct);
            if (!ok) throw new UnauthorizedAccessException("Credenciales inválidas.");

            var usuario = await _usuarioRepo.ObtenerPorNombreUsuarioAsync(username, ct);
            if (usuario == null || !usuario.Activo)
                throw new UnauthorizedAccessException("Usuario no encontrado o inactivo.");

            return usuario;
        }

        public Task<bool> ValidarUsuarioAsync(string username, string password, CancellationToken ct = default)
            => _usuarioRepo.VerificarCredencialesAsync(username, password, ct);

        public Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default)
            => _usuarioRepo.ObtenerPorNombreUsuarioAsync(nombreUsuario, ct);

        public Task<IReadOnlyList<Rol>> ObtenerRolesDeUsuarioAsync(int usuarioId, CancellationToken ct = default)
            => _usuarioRepo.ObtenerRolesDeUsuarioAsync(usuarioId, ct);

        public Task<IReadOnlyList<Usuario>> BuscarPorRolIdAsync(int rolId, CancellationToken ct = default)
            => _usuarioRepo.BuscarPorRolIdAsync(rolId, ct);

        public async Task CrearUsuarioAdministradorSiNoExisteAsync(CancellationToken ct = default)
        {
            // Asegurar rol "Administrador"
            var rolAdmin = await _rolRepo.ObtenerPorNombreAsync("Administrador", ct);
            if (rolAdmin == null)
            {
                rolAdmin = await _rolRepo.GuardarAsync(new Rol { Nombre = "Administrador" }, ct);
            }

            // Usuario admin por defecto
            const string adminUser = "admin";
            const string adminPass = "admin123";
            var usuario = await _usuarioRepo.ObtenerPorNombreUsuarioAsync(adminUser, ct);

            if (usuario == null)
            {
                usuario = new Usuario
                {
                    NombreUsuario = adminUser,
                    NombreCompleto = "Administrador del Sistema",
                    PasswordHash = _hasher.HashPassword(adminPass),
                    Activo = true
                };

                var nuevoId = await _usuarioRepo.CrearUsuarioAsync(usuario, ct);
                usuario.Id = nuevoId;
            }

            // Asegurar asignación de rol
            var rolesUsuario = await _usuarioRepo.ObtenerRolesDeUsuarioAsync(usuario.Id, ct);
            if (!rolesUsuario.Any(r => r.Id == rolAdmin.Id))
            {
                await _usuarioRepo.AsignarRolUsuarioAsync(usuario.Id, rolAdmin.Id, ct);
            }
        }

        public async Task<Usuario> RegistrarUsuarioAsync(Usuario usuario, string password, int rolId, CancellationToken ct = default)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("La contraseña es obligatoria.", nameof(password));

            usuario.NombreUsuario = usuario.NombreUsuario?.Trim() ?? string.Empty;
            usuario.NombreCompleto = usuario.NombreCompleto?.Trim() ?? usuario.NombreUsuario;
            usuario.PasswordHash = _hasher.HashPassword(password);
            usuario.Activo = true;

            var id = await _usuarioRepo.CrearUsuarioAsync(usuario, ct);
            usuario.Id = id;

            await _usuarioRepo.AsignarRolUsuarioAsync(usuario.Id, rolId, ct);
            return usuario;
        }
    }
}