using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.DTOs;
using InventarioComputo.Domain.Entities;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using Color = System.Drawing.Color; // Resolver ambigüedad especificando el alias

namespace InventarioComputo.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de reportes para exportar a Excel y PDF
    /// </summary>
    public class ReporteService : IReporteService
    {
        private readonly IEquipoComputoRepository _equipoRepo;
        private readonly ILogger<ReporteService> _logger;

        public ReporteService(IEquipoComputoRepository equipoRepo, ILogger<ReporteService> logger)
        {
            _equipoRepo = equipoRepo;
            _logger = logger;
            
            // Configurar licencia no comercial de EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Obtiene los equipos filtrados según criterios para reportes
        /// </summary>
        public async Task<IReadOnlyList<EquipoComputo>> ObtenerEquiposFiltradosAsync(
            FiltroReporteDTO filtro, CancellationToken ct = default)
        {
            try
            {
                return await _equipoRepo.ObtenerParaReporteAsync(filtro, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener equipos filtrados para reporte");
                throw;
            }
        }

        /// <summary>
        /// Obtiene los equipos filtrados y los transforma a DTOs para reportes
        /// </summary>
        public async Task<IReadOnlyList<ReporteEquipoDTO>> ObtenerEquiposDTOFiltradosAsync(
            FiltroReporteDTO filtro, CancellationToken ct = default)
        {
            try
            {
                var equipos = await _equipoRepo.ObtenerParaReporteAsync(filtro, ct);
                return equipos.Select(MapearAReporteDTO).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener equipos DTO filtrados para reporte");
                throw;
            }
        }

        /// <summary>
        /// Exporta los datos a un archivo Excel
        /// </summary>
        public async Task<byte[]> ExportarExcelAsync(
            IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Iniciando exportación a Excel con {Count} registros", datos.Count);
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Inventario");

                // Título del reporte
                worksheet.Cells["A1:J1"].Merge = true;
                worksheet.Cells["A1"].Value = "REPORTE DE INVENTARIO DE EQUIPOS DE CÓMPUTO";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                
                // Fecha de generación
                worksheet.Cells["A2:J2"].Merge = true;
                worksheet.Cells["A2"].Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
                worksheet.Cells["A2"].Style.Font.Size = 11;
                worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                
                // Espacio
                worksheet.Cells["A3:J3"].Merge = true;

                // Encabezados con estilo
                var headers = new string[]
                {
                    "ID", "Número Serie", "Etiqueta", "Marca", "Modelo", "Tipo Equipo",
                    "Estado", "Usuario Asignado", "Ubicación", "Fecha Adquisición"
                };

                // Aplicar encabezados
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[4, i + 1].Value = headers[i];
                }

                // Dar formato a encabezados
                using (var headerRange = worksheet.Cells[4, 1, 4, headers.Length])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                    headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Agregar datos
                for (int i = 0; i < datos.Count; i++)
                {
                    var row = i + 5; // Empezamos en la fila 5 (después del título, fecha y encabezados)
                    var item = datos[i];
                    
                    worksheet.Cells[row, 1].Value = item.Id;
                    worksheet.Cells[row, 2].Value = item.NumeroSerie;
                    worksheet.Cells[row, 3].Value = item.EtiquetaInventario;
                    worksheet.Cells[row, 4].Value = item.Marca;
                    worksheet.Cells[row, 5].Value = item.Modelo;
                    worksheet.Cells[row, 6].Value = item.TipoEquipo;
                    worksheet.Cells[row, 7].Value = item.Estado;
                    worksheet.Cells[row, 8].Value = item.UsuarioAsignado;
                    worksheet.Cells[row, 9].Value = item.Ubicacion;
                    
                    if (item.FechaAdquisicion != DateTime.MinValue)
                    {
                        worksheet.Cells[row, 10].Value = item.FechaAdquisicion;
                        worksheet.Cells[row, 10].Style.Numberformat.Format = "dd/mm/yyyy";
                    }
                    
                    // Agregar borde a las celdas
                    using (var dataRange = worksheet.Cells[row, 1, row, headers.Length])
                    {
                        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                }

                // Información de totales
                var lastRow = datos.Count + 5;
                worksheet.Cells[$"A{lastRow + 1}:B{lastRow + 1}"].Merge = true;
                worksheet.Cells[$"A{lastRow + 1}"].Value = "Total de equipos:";
                worksheet.Cells[$"A{lastRow + 1}"].Style.Font.Bold = true;
                worksheet.Cells[$"A{lastRow + 1}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                
                worksheet.Cells[$"C{lastRow + 1}"].Value = datos.Count;
                
                // Auto ajustar columnas
                worksheet.Cells.AutoFitColumns();
                
                // Definir ancho mínimo para ciertas columnas
                worksheet.Column(9).Width = Math.Max(worksheet.Column(9).Width, 30); // Ubicación
                
                _logger.LogInformation("Exportación a Excel completada");
                return await package.GetAsByteArrayAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar a Excel");
                throw new InvalidOperationException("Error al generar el archivo Excel: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Exporta los datos a un archivo PDF
        /// </summary>
        public async Task<byte[]> ExportarPDFAsync(
            IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Iniciando exportación a PDF con {Count} registros", datos.Count);
                using var stream = new MemoryStream();
                using var writer = new PdfWriter(stream);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf);

                // Título del documento
                var title = new Paragraph("REPORTE DE INVENTARIO DE EQUIPOS DE CÓMPUTO")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(16)
                    .SetBold();
                document.Add(title);

                // Fecha de generación
                var fechaGeneracion = new Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(11);
                document.Add(fechaGeneracion);

                document.Add(new Paragraph("\n")); // Espacio

                // Crear tabla
                var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2, 2, 2, 2, 2, 2, 2, 3, 2 }))
                    .UseAllAvailableWidth();

                // Encabezados
                string[] headers = { 
                    "ID", "No. Serie", "Etiqueta", "Marca", "Modelo", 
                    "Tipo", "Estado", "Usuario", "Ubicación", "F. Adquisición"
                };

                // Estilo para encabezados
                foreach (var header in headers)
                {
                    var cell = new Cell()
                        .Add(new Paragraph(header))
                        .SetBackgroundColor(new DeviceRgb(176, 196, 222)) // LightSteelBlue
                        .SetBold()
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(5);
                    table.AddHeaderCell(cell);
                }

                // Agregar datos
                foreach (var item in datos)
                {
                    // ID
                    table.AddCell(new Cell().Add(new Paragraph(item.Id.ToString())).SetTextAlignment(TextAlignment.CENTER));
                    
                    // Número Serie
                    table.AddCell(new Cell().Add(new Paragraph(item.NumeroSerie)));
                    
                    // Etiqueta
                    table.AddCell(new Cell().Add(new Paragraph(item.EtiquetaInventario)));
                    
                    // Marca
                    table.AddCell(new Cell().Add(new Paragraph(item.Marca)));
                    
                    // Modelo
                    table.AddCell(new Cell().Add(new Paragraph(item.Modelo)));
                    
                    // Tipo Equipo
                    table.AddCell(new Cell().Add(new Paragraph(item.TipoEquipo)));
                    
                    // Estado
                    table.AddCell(new Cell().Add(new Paragraph(item.Estado)));
                    
                    // Usuario Asignado
                    table.AddCell(new Cell().Add(new Paragraph(item.UsuarioAsignado ?? "")));
                    
                    // Ubicación
                    table.AddCell(new Cell().Add(new Paragraph(item.Ubicacion ?? "")));
                    
                    // Fecha Adquisición
                    var fechaAdq = item.FechaAdquisicion != DateTime.MinValue 
                        ? item.FechaAdquisicion.ToString("dd/MM/yyyy") 
                        : "";
                    table.AddCell(new Cell().Add(new Paragraph(fechaAdq)).SetTextAlignment(TextAlignment.CENTER));
                }

                document.Add(table);

                // Pie del reporte
                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph($"Total de equipos: {datos.Count}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBold());

                document.Add(new Paragraph("\n"));
                document.Add(new Paragraph("Henniges Automotive Planta 1 - Sistema de Inventario")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10)
                    .SetItalic());

                document.Close();
                _logger.LogInformation("Exportación a PDF completada");
                
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar a PDF");
                throw new InvalidOperationException("Error al generar el archivo PDF: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Mapea una entidad EquipoComputo a un DTO para reportes
        /// </summary>
        private static ReporteEquipoDTO MapearAReporteDTO(EquipoComputo equipo)
        {
            string ObtenerUbicacionCompleta(EquipoComputo e)
            {
                var partes = new List<string>();
                
                // Sede (directa o desde zona->area->sede)
                string sede = e.Sede?.Nombre ?? e.Zona?.Area?.Sede?.Nombre ?? string.Empty;
                if (!string.IsNullOrEmpty(sede))
                    partes.Add(sede);
                    
                // Área (directa o desde zona->area)
                string area = e.Area?.Nombre ?? e.Zona?.Area?.Nombre ?? string.Empty;
                if (!string.IsNullOrEmpty(area) && area != sede)
                    partes.Add(area);
                    
                // Zona
                if (e.Zona != null && !string.IsNullOrEmpty(e.Zona.Nombre))
                    partes.Add(e.Zona.Nombre);
                    
                return string.Join(" / ", partes);
            }

            return new ReporteEquipoDTO
            {
                Id = equipo.Id,
                NumeroSerie = equipo.NumeroSerie,
                EtiquetaInventario = equipo.EtiquetaInventario,
                Marca = equipo.Marca,
                Modelo = equipo.Modelo,
                TipoEquipo = equipo.TipoEquipo?.Nombre ?? string.Empty,
                Estado = equipo.Estado?.Nombre ?? string.Empty,
                Usuario = string.Empty, // Campo aparentemente no usado
                UsuarioAsignado = equipo.Empleado?.NombreCompleto ?? string.Empty,
                Sede = equipo.Sede?.Nombre ?? equipo.Zona?.Area?.Sede?.Nombre ?? string.Empty,
                Area = equipo.Area?.Nombre ?? equipo.Zona?.Area?.Nombre ?? string.Empty,
                Zona = equipo.Zona?.Nombre ?? string.Empty,
                Ubicacion = ObtenerUbicacionCompleta(equipo),
                FechaAdquisicion = equipo.FechaAdquisicion ?? DateTime.MinValue,
                Costo = equipo.Costo,
                Activo = equipo.Activo
            };
        }
    }
}