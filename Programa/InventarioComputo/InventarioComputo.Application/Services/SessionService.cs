using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUsuarioService _usuarios;
        private readonly ILogger<SessionService>? _logger;

        public event EventHandler<bool>? SesionCambiada;

        public Usuario? UsuarioActual { get; private set; }
        public bool EstaAutenticado => UsuarioActual != null;

        private IReadOnlyList<Rol> _roles = Array.Empty<Rol>();

        public SessionService(IUsuarioService usuarios, ILogger<SessionService>? logger = null)
        {
            _usuarios = usuarios;
            _logger = logger;
        }

        public async Task<bool> IniciarSesionAsync(string usuario, string password)
        {
            try
            {
                var ok = await _usuarios.AuthenticateAsync(usuario, password);
                if (!ok) return false;

                var u = await _usuarios.ObtenerPorNombreUsuarioAsync(usuario);
                if (u == null || !u.Activo) return false;

                UsuarioActual = u;
                _roles = await _usuarios.ObtenerRolesDeUsuarioAsync(u.Id);
                SesionCambiada?.Invoke(this, true);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al iniciar sesión");
                return false;
            }
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
            _roles = Array.Empty<Rol>();
            SesionCambiada?.Invoke(this, false);
        }

        public bool TieneRol(string rolNombre)
        {
            if (!EstaAutenticado) return false;

            string normalizado = NormalizarRol(rolNombre);
            return _roles.Any(r =>
                string.Equals(r.Nombre, rolNombre, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.Nombre, normalizado, StringComparison.OrdinalIgnoreCase));
        }

        public IReadOnlyList<Rol> ObtenerRolesUsuario() => _roles;

        private static string NormalizarRol(string nombre) => nombre switch
        {
            "Administrador" => "Administradores",
            "Admin" => "Administradores",
            "Consultas" => "Consulta",
            _ => nombre
        };
    }
}