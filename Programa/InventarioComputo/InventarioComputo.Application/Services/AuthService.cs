using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRolRepository _rolRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IRolRepository rolRepository,
            IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Usuario> AuthenticateAsync(string username, string password, CancellationToken ct = default)
        {
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(username, ct);
            if (usuario == null || !_passwordHasher.VerifyPassword(password, usuario.PasswordHash))
                throw new UnauthorizedAccessException("Credenciales inválidas");
            
            return usuario;
        }

        public async Task<bool> ValidarUsuarioAsync(string username, string password, CancellationToken ct = default)
        {
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(username, ct);
            return usuario != null && _passwordHasher.VerifyPassword(password, usuario.PasswordHash);
        }

        public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default)
        {
            return await _usuarioRepository.ObtenerPorNombreUsuarioAsync(nombreUsuario, ct);
        }

        public async Task<IReadOnlyList<Rol>> ObtenerRolesDeUsuarioAsync(int usuarioId, CancellationToken ct = default)
        {
            return await _rolRepository.ObtenerRolesPorUsuarioIdAsync(usuarioId, ct);
        }

        public async Task CrearUsuarioAdministradorSiNoExisteAsync(CancellationToken ct = default)
        {
            const string adminUser = "admin";
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(adminUser, ct);
            
            if (usuario != null) 
                return;

            // Buscar o crear el rol de administrador
            var adminRol = await _rolRepository.ObtenerPorNombreAsync("Administrador", ct);
            int adminRolId;
            
            if (adminRol == null)
            {
                adminRolId = await _rolRepository.CrearRolAsync(new Rol { Nombre = "Administrador" }, ct);
            }
            else
            {
                adminRolId = adminRol.Id;
            }

            // Crear usuario admin
            var nuevoAdmin = new Usuario
            {
                NombreUsuario = adminUser,
                NombreCompleto = "Administrador del Sistema",
                PasswordHash = _passwordHasher.HashPassword("admin123"),
                Activo = true
            };

            int nuevoUsuarioId = await _usuarioRepository.CrearUsuarioAsync(nuevoAdmin, ct);
            
            // Asignar rol admin
            await _rolRepository.AsignarRolAUsuarioAsync(nuevoUsuarioId, adminRolId, ct);
        }

        public async Task<IReadOnlyList<Usuario>> BuscarPorRolIdAsync(int rolId, CancellationToken ct = default)
        {
            return await _usuarioRepository.BuscarPorRolIdAsync(rolId, ct);
        }

        public async Task<Usuario> RegistrarUsuarioAsync(Usuario usuario, string password, int rolId, CancellationToken ct = default)
        {
            // Verificar si ya existe un usuario con ese nombre
            var existente = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(usuario.NombreUsuario, ct);
            if (existente != null)
                throw new InvalidOperationException($"Ya existe un usuario con el nombre '{usuario.NombreUsuario}'");

            // Hashear la contraseña y asignarla
            usuario.PasswordHash = _passwordHasher.HashPassword(password);
            
            // Crear el usuario
            int userId = await _usuarioRepository.CrearUsuarioAsync(usuario, ct);
            
            // Asignar el rol
            await _rolRepository.AsignarRolAUsuarioAsync(userId, rolId, ct);
            
            // Devolver el usuario con el ID asignado
            usuario.Id = userId;
            return usuario;
        }
    }
}