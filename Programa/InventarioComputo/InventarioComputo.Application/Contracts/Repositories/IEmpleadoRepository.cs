using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IEmpleadoRepository
    {
        Task<IReadOnlyList<Empleado>> BuscarAsync(string? filtro, bool incluirInactivos = false, CancellationToken ct = default);
        Task<IReadOnlyList<Empleado>> ObtenerTodosAsync(bool incluirInactivos = false, CancellationToken ct = default);
        Task<Empleado?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<bool> ExisteNombreAsync(string nombreCompleto, int? excluirId = null, CancellationToken ct = default);
        Task<Empleado> GuardarAsync(Empleado entidad, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}