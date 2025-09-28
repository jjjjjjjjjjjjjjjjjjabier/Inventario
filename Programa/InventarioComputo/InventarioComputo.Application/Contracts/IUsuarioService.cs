using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IUsuarioService
    {
        Task<bool> AuthenticateAsync(string nombreUsuario, string password, CancellationToken ct = default);
        Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken ct = default);
        Task<IReadOnlyList<Rol>> ObtenerRolesDeUsuarioAsync(int usuarioId, CancellationToken ct = default);
        Task<IReadOnlyList<Usuario>> BuscarAsync(string? filtro, bool incluirInactivos = false, CancellationToken ct = default);
        Task<Usuario?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Usuario> GuardarAsync(Usuario usuario, string? password = null, CancellationToken ct = default);
        Task AsignarRolUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default);
        Task QuitarRolUsuarioAsync(int usuarioId, int rolId, CancellationToken ct = default);
        Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idExcluir = null, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}