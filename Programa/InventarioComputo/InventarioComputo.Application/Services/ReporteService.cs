using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.DTOs;
using InventarioComputo.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventarioComputo.Application.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IEquipoComputoRepository _equipoRepo;

        public ReporteService(IEquipoComputoRepository equipoRepo)
        {
            _equipoRepo = equipoRepo;
        }

        public async Task<IReadOnlyList<EquipoComputo>> ObtenerEquiposFiltradosAsync(FiltroReporteDTO filtro, CancellationToken ct = default)
        {
            // Este método puede usar tu repositorio actual para traer datos,
            // lo dejamos como está si ya funciona. Ajusta si es necesario.
            var lista = await _equipoRepo.ObtenerParaReporteAsync(filtro, ct);
            return lista;
        }

        public Task<byte[]> ExportarExcelAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            // Implementación existente
            return Task.FromResult(System.Array.Empty<byte>());
        }

        public Task<byte[]> ExportarPDFAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            // Implementación existente
            return Task.FromResult(System.Array.Empty<byte>());
        }

        // Helper de mapeo (si lo usas)
        private static ReporteEquipoDTO Map(EquipoComputo e)
        {
            return new ReporteEquipoDTO
            {
                Id = e.Id,
                NumeroSerie = e.NumeroSerie,
                EtiquetaInventario = e.EtiquetaInventario,
                Marca = e.Marca,
                Modelo = e.Modelo,
                TipoEquipo = e.TipoEquipo?.Nombre ?? string.Empty,
                Estado = e.Estado?.Nombre ?? string.Empty,
                Usuario = e.Empleado?.NombreCompleto ?? string.Empty,     // <-- Antes: e.Usuario?.NombreCompleto
                UsuarioAsignado = e.Empleado?.NombreCompleto ?? string.Empty,
                Sede = e.Sede?.Nombre ?? string.Empty,
                Area = e.Area?.Nombre ?? string.Empty,
                Zona = e.Zona?.Nombre ?? string.Empty,
                Ubicacion = e.Zona?.Nombre ?? string.Empty,
                FechaAdquisicion = e.FechaAdquisicion ?? default,
                Costo = e.Costo,
                Activo = e.Activo
            };
        }
    }
}