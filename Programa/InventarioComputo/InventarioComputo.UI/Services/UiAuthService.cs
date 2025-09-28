using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;

namespace InventarioComputo.UI.Services
{
    // Wrapper/fachada para reutilizar el servicio real de autenticación
    public class UiAuthService : IAuthService
    {
        private readonly IAuthService _inner;

        public UiAuthService(IAuthService inner)
        {
            _inner = inner;
        }

        public Task<Usuario> AuthenticateAsync(string username, string password, CancellationToken ct = default)
            => _inner.AuthenticateAsync(username, password, ct);

        public Task<bool> ValidarUsuarioAsync(string username, string password, CancellationToken ct = default)
            => _inner.ValidarUsuarioAsync(username, password, ct);

        public Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default)
            => _inner.ObtenerPorNombreUsuarioAsync(nombreUsuario, ct);

        public Task<IReadOnlyList<Rol>> ObtenerRolesDeUsuarioAsync(int usuarioId, CancellationToken ct = default)
            => _inner.ObtenerRolesDeUsuarioAsync(usuarioId, ct);

        public Task<IReadOnlyList<Usuario>> BuscarPorRolIdAsync(int rolId, CancellationToken ct = default)
            => _inner.BuscarPorRolIdAsync(rolId, ct);

        public Task CrearUsuarioAdministradorSiNoExisteAsync(CancellationToken ct = default)
            => _inner.CrearUsuarioAdministradorSiNoExisteAsync(ct);

        public Task<Usuario> RegistrarUsuarioAsync(Usuario usuario, string password, int rolId, CancellationToken ct = default)
            => _inner.RegistrarUsuarioAsync(usuario, password, rolId, ct);
    }
}