using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IEmpleadoService
    {
        Task<IReadOnlyList<Empleado>> BuscarAsync(string? filtro, bool incluirInactivos, CancellationToken ct = default);
        Task<Empleado?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<Empleado> GuardarAsync(Empleado entidad, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
    }
}