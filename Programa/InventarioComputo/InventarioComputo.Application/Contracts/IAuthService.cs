using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IAuthService
    {
        Task<Usuario> AuthenticateAsync(string username, string password, CancellationToken ct = default);
        Task<bool> ValidarUsuarioAsync(string username, string password, CancellationToken ct = default);
        Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default);
        Task<IReadOnlyList<Rol>> ObtenerRolesDeUsuarioAsync(int usuarioId, CancellationToken ct = default);
        Task<IReadOnlyList<Usuario>> BuscarPorRolIdAsync(int rolId, CancellationToken ct = default);
        Task CrearUsuarioAdministradorSiNoExisteAsync(CancellationToken ct = default);
        Task<Usuario> RegistrarUsuarioAsync(Usuario usuario, string password, int rolId, CancellationToken ct = default);
    }
}