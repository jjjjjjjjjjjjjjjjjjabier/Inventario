using InventarioComputo.Domain.DTOs;
using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts.Repositories
{
    public interface IEquipoComputoRepository
    {
        Task<IReadOnlyList<EquipoComputo>> ObtenerTodosAsync(bool incluirInactivos, CancellationToken ct = default);
        Task<IReadOnlyList<EquipoComputo>> BuscarAsync(string? filtro, bool incluirInactivos, CancellationToken ct = default);
        Task<EquipoComputo?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
        Task<EquipoComputo> AgregarAsync(EquipoComputo equipo, CancellationToken ct = default);
        Task<EquipoComputo> ActualizarAsync(EquipoComputo equipo, CancellationToken ct = default);
        Task EliminarAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsByNumeroSerieAsync(string numeroSerie, int? idExcluir = null, CancellationToken ct = default);
        Task<bool> ExistsByEtiquetaInventarioAsync(string etiqueta, int? idExcluir = null, CancellationToken ct = default);
        Task<IReadOnlyList<EquipoComputo>> ObtenerParaReporteAsync(FiltroReporteDTO filtro, CancellationToken ct = default);
    }
}