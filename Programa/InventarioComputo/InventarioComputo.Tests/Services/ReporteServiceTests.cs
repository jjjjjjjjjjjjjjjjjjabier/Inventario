using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Application.Services;
using InventarioComputo.Domain.DTOs;
using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace InventarioComputo.Tests.Services
{
    public class ReporteServiceTests
    {
        private class EquipoRepoFake : IEquipoComputoRepository
        {
            public Task<EquipoComputo> AgregarAsync(EquipoComputo equipo, CancellationToken ct = default) => Task.FromResult(equipo);
            public Task ActualizarAsync(EquipoComputo equipo, CancellationToken ct = default) => Task.FromResult(equipo);
            public Task EliminarAsync(int id, CancellationToken ct = default) => Task.CompletedTask;
            public Task<bool> ExistsByEtiquetaInventarioAsync(string etiqueta, int? idExcluir = null, CancellationToken ct = default) => Task.FromResult(false);
            public Task<bool> ExistsByNumeroSerieAsync(string numeroSerie, int? idExcluir = null, CancellationToken ct = default) => Task.FromResult(false);
            public Task<IReadOnlyList<EquipoComputo>> BuscarAsync(string? filtro, bool incluirInactivos, CancellationToken ct = default) => Task.FromResult((IReadOnlyList<EquipoComputo>)new List<EquipoComputo>());
            public Task<EquipoComputo?> ObtenerPorIdAsync(int id, CancellationToken ct = default) => Task.FromResult<EquipoComputo?>(null);
            public Task<IReadOnlyList<EquipoComputo>> ObtenerTodosAsync(bool incluirInactivos, CancellationToken ct = default) => Task.FromResult((IReadOnlyList<EquipoComputo>)new List<EquipoComputo>());
            public Task<IReadOnlyList<EquipoComputo>> ObtenerParaReporteAsync(FiltroReporteDTO filtro, CancellationToken ct = default) => Task.FromResult((IReadOnlyList<EquipoComputo>)new List<EquipoComputo>());
            public Task<EquipoComputo> ActualizarAsync(EquipoComputo equipo, CancellationToken ct = default) => Task.FromResult(equipo);
        }

        [Fact]
        public async Task ExportarPDFAsync_GeneraPdfConCabeceraValida()
        {
            var repo = new EquipoRepoFake();
            var srv = new ReporteService(repo);

            var datos = new List<ReporteEquipoDTO>
            {
                new ReporteEquipoDTO
                {
                    EtiquetaInventario = "INV-001",
                    NumeroSerie = "SER-001",
                    Marca = "Dell",
                    Modelo = "OptiPlex",
                    TipoEquipo = "Desktop",
                    Estado = "Operativo",
                    UsuarioAsignado = "Juan Pérez",
                    Ubicacion = "Sede A > Área TI > Zona 1",
                    Activo = true
                }
            };

            var bytes = await srv.ExportarPDFAsync(datos);
            Assert.NotNull(bytes);
            Assert.True(bytes.Length > 1000, "El PDF debe tener contenido.");
            Assert.StartsWith("%PDF-", System.Text.Encoding.ASCII.GetString(bytes, 0, 5));
        }
    }
}