using InventarioComputo.Domain.DTOs;
using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Contracts
{
    public interface IReporteService
    {
        Task<IReadOnlyList<EquipoComputo>> ObtenerEquiposFiltradosAsync(FiltroReporteDTO filtro, CancellationToken ct = default);
        Task<byte[]> ExportarExcelAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default);
        Task<byte[]> ExportarPDFAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default);
    }
}