using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IUsuarioRepository
    {
        Task<IReadOnlyList<Usuario>> BuscarAsync(string? filtro, bool incluirInactivos, CancellationToken ct = default);

        // Dos firmas para dar compatibilidad a llamadas existentes
        Task<IReadOnlyList<Usuario>> BuscarPorRolIdAsync(int rolId, bool incluirInactivos, CancellationToken ct = default);
        Task<IReadOnlyList<Usuario>> BuscarPorRolIdAsync(int rolId, CancellationToken ct = default);

        Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default);
        Task<Usuario> GuardarAsync(Usuario usuario, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);

        // Autenticación/validación
        Task<bool> AuthenticateAsync(string nombreUsuario, string password, CancellationToken ct = default);
        Task<bool> VerificarCredencialesAsync(string nombreUsuario, string password, CancellationToken ct = default);

        // Duplicados
        Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idExcluir = null, CancellationToken ct = default);

        // Roles
        Task<IReadOnlyList<Rol>> ObtenerRolesDeUsuarioAsync(int usuarioId, CancellationToken ct = default);
        Task AsignarRolUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default);
        Task QuitarRolUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default);
        Task<int> CrearUsuarioAsync(Usuario usuario, CancellationToken ct = default);
    }
}