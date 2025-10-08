using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IRolRepository _rolRepo;
        private readonly IAuthService _authService;

        private Usuario? _usuarioActual;
        private List<Rol> _roles = new();

        public event EventHandler<bool>? SesionCambiada;

        public Usuario? UsuarioActual => _usuarioActual;
        public bool EstaAutenticado => _usuarioActual != null;

        public SessionService(IUsuarioRepository usuarioRepo, IRolRepository rolRepo, IAuthService authService)
        {
            _usuarioRepo = usuarioRepo;
            _rolRepo = rolRepo;
            _authService = authService;
        }

        public async Task<bool> IniciarSesionAsync(string usuario, string password)
        {
            var resultado = await _authService.ValidarUsuarioAsync(usuario, password);
            if (resultado)
            {
                _usuarioActual = await _usuarioRepo.ObtenerPorNombreUsuarioAsync(usuario);
                if (_usuarioActual != null)
                {
                    _roles = (await _rolRepo.ObtenerRolesPorUsuarioIdAsync(_usuarioActual.Id)).ToList();
                    SesionCambiada?.Invoke(this, true);
                    return true;
                }
            }
            return false;
        }

        public void CerrarSesion()
        {
            _usuarioActual = null;
            _roles.Clear();
            SesionCambiada?.Invoke(this, false);
        }

        private static string NormalizarRol(string nombre)
        {
            var n = (nombre ?? string.Empty).Trim().ToLowerInvariant();
            return n switch
            {
                "admin" or "administrador" or "administradores" => "administradores",
                "consulta" or "consultas" => "consulta",
                "soporte" => "soporte",
                _ => n
            };
        }

        public bool TieneRol(string rolNombre)
        {
            if (_usuarioActual is null) return false;

            var buscado = NormalizarRol(rolNombre);
            return _roles.Any(r => NormalizarRol(r.Nombre) == buscado);
        }

        public IReadOnlyList<Rol> ObtenerRolesUsuario() => _roles.AsReadOnly();
    }
}