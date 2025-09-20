using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IEquipoComputoRepository
    {
        Task<IReadOnlyList<EquipoComputo>> ObtenerTodosAsync(CancellationToken ct);
        Task<EquipoComputo?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<EquipoComputo> AgregarAsync(EquipoComputo equipo, CancellationToken ct);
        Task ActualizarAsync(EquipoComputo equipo, CancellationToken ct);
        Task EliminarAsync(int id, CancellationToken ct);
        Task<bool> ExistsByNumeroSerieAsync(string numeroSerie, int? idExcluir, CancellationToken ct);
    }
}