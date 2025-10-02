using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.DTOs;
using InventarioComputo.Domain.Entities;
using System;
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

        public async Task<IReadOnlyList<EquipoComputo>> ObtenerEquiposFiltradosAsync(
            FiltroReporteDTO filtro, CancellationToken ct = default)
        {
            return await _equipoRepo.ObtenerParaReporteAsync(filtro, ct);
        }

        public async Task<byte[]> ExportarExcelAsync(
            IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            // Implementación para generar Excel
            // Este es un ejemplo simplificado - usa una librería como EPPlus para implementación real
            
            // Simulamos la exportación para que compile
            var result = new byte[1];
            return await Task.FromResult(result);
        }

        public async Task<byte[]> ExportarPDFAsync(
            IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            // Implementación para generar PDF
            // Este es un ejemplo simplificado - usa una librería como iTextSharp para implementación real
            
            // Simulamos la exportación para que compile
            var result = new byte[1];
            return await Task.FromResult(result);
        }

        public Task<IReadOnlyList<ReporteEquipoDTO>> MapearEquiposADTOsAsync(
            IReadOnlyList<EquipoComputo> equipos)
        {
            var result = equipos.Select(e => new ReporteEquipoDTO
            {
                Id = e.Id,
                NumeroSerie = e.NumeroSerie,
                EtiquetaInventario = e.EtiquetaInventario,
                Marca = e.Marca,
                Modelo = e.Modelo,
                TipoEquipo = e.TipoEquipo?.Nombre ?? string.Empty,
                Estado = e.Estado?.Nombre ?? string.Empty,
                Usuario = e.Usuario?.NombreCompleto ?? string.Empty,
                Sede = e.Zona?.Area?.Sede?.Nombre ?? string.Empty,
                Area = e.Zona?.Area?.Nombre ?? string.Empty,
                Zona = e.Zona?.Nombre ?? string.Empty,
                UsuarioAsignado = e.Usuario?.NombreCompleto ?? string.Empty,
                Ubicacion = ObtenerUbicacionFormateada(e),
                // Corregido para manejar DateTime nullable
                FechaAdquisicion = e.FechaAdquisicion ?? DateTime.MinValue,
                Costo = e.Costo,
                Activo = e.Activo
            }).ToList();

            return Task.FromResult<IReadOnlyList<ReporteEquipoDTO>>(result);
        }

        private string ObtenerUbicacionFormateada(EquipoComputo e)
        {
            var ubicacion = string.Empty;

            if (e.Zona?.Area?.Sede != null)
                ubicacion = $"{e.Zona.Area.Sede.Nombre}";

            if (e.Zona?.Area != null)
                ubicacion += $" - {e.Zona.Area.Nombre}";

            if (e.Zona != null)
                ubicacion += $" - {e.Zona.Nombre}";

            return ubicacion;
        }
    }
}