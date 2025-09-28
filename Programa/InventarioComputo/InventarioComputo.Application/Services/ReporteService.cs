using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.DTOs;
using InventarioComputo.Domain.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            return await _equipoRepo.ObtenerParaReporteAsync(filtro, ct);
        }

        public async Task<byte[]> ExportarExcelAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            // Configurar licencia de EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Inventario Equipos");

                // Encabezados
                worksheet.Cells[1, 1].Value = "Etiqueta";
                worksheet.Cells[1, 2].Value = "Número Serie";
                worksheet.Cells[1, 3].Value = "Marca";
                worksheet.Cells[1, 4].Value = "Modelo";
                worksheet.Cells[1, 5].Value = "Tipo";
                worksheet.Cells[1, 6].Value = "Estado";
                worksheet.Cells[1, 7].Value = "Ubicación";
                worksheet.Cells[1, 8].Value = "Usuario Asignado";
                worksheet.Cells[1, 9].Value = "Fecha Adquisición";
                worksheet.Cells[1, 10].Value = "Activo";

                // Formatear encabezados
                using (var headerRange = worksheet.Cells[1, 1, 1, 10])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                // Datos
                for (int i = 0; i < datos.Count; i++)
                {
                    var equipo = datos[i];
                    int row = i + 2;

                    worksheet.Cells[row, 1].Value = equipo.EtiquetaInventario;
                    worksheet.Cells[row, 2].Value = equipo.NumeroSerie;
                    worksheet.Cells[row, 3].Value = equipo.Marca;
                    worksheet.Cells[row, 4].Value = equipo.Modelo;
                    worksheet.Cells[row, 5].Value = equipo.TipoEquipo;
                    worksheet.Cells[row, 6].Value = equipo.Estado;
                    worksheet.Cells[row, 7].Value = equipo.Ubicacion;
                    worksheet.Cells[row, 8].Value = equipo.UsuarioAsignado;

                    if (equipo.FechaAdquisicion.HasValue)
                    {
                        worksheet.Cells[row, 9].Value = equipo.FechaAdquisicion.Value;
                        worksheet.Cells[row, 9].Style.Numberformat.Format = "dd/mm/yyyy";
                    }

                    worksheet.Cells[row, 10].Value = equipo.Activo ? "Sí" : "No";
                }

                // Autoajustar columnas
                worksheet.Cells.AutoFitColumns();

                // Configurar filtros automáticos
                var dataRange = worksheet.Cells[1, 1, datos.Count + 1, 10];
                dataRange.AutoFilter = true;

                // Devolver el archivo como array de bytes
                return await package.GetAsByteArrayAsync(ct);
            }
        }

        public async Task<byte[]> ExportarPDFAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            // Este método requeriría una librería como iText7 o QuestPDF para implementarse correctamente
            // Por ahora, implementamos una versión simple generando HTML y convirtiéndolo a PDF con una librería externa
            // o usando una herramienta de terceros.

            // Nota: Este es un ejemplo simplificado, necesitarías instalar una librería para PDF
            string html = GenerarHTMLReporte(datos);

            // Convertir HTML a PDF (usando alguna biblioteca como SelectPdf, DinkToPdf, etc.)
            // Por el momento, retornamos una representación del HTML como bytes
            return System.Text.Encoding.UTF8.GetBytes(html);
        }

        private string GenerarHTMLReporte(IReadOnlyList<ReporteEquipoDTO> datos)
        {
            var html = new System.Text.StringBuilder();
            html.Append("<html><head>");
            html.Append("<style>");
            html.Append("body { font-family: Arial, sans-serif; }");
            html.Append("table { width: 100%; border-collapse: collapse; }");
            html.Append("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            html.Append("th { background-color: #f2f2f2; }");
            html.Append("</style>");
            html.Append("</head><body>");
            html.Append("<h1>Reporte de Equipos</h1>");
            html.Append("<table>");
            html.Append("<tr><th>Etiqueta</th><th>Número Serie</th><th>Marca</th><th>Modelo</th><th>Tipo</th>");
            html.Append("<th>Estado</th><th>Ubicación</th><th>Usuario</th><th>F. Adquisición</th><th>Activo</th></tr>");

            foreach (var equipo in datos)
            {
                html.Append("<tr>");
                html.AppendFormat("<td>{0}</td>", equipo.EtiquetaInventario);
                html.AppendFormat("<td>{0}</td>", equipo.NumeroSerie);
                html.AppendFormat("<td>{0}</td>", equipo.Marca);
                html.AppendFormat("<td>{0}</td>", equipo.Modelo);
                html.AppendFormat("<td>{0}</td>", equipo.TipoEquipo);
                html.AppendFormat("<td>{0}</td>", equipo.Estado);
                html.AppendFormat("<td>{0}</td>", equipo.Ubicacion);
                html.AppendFormat("<td>{0}</td>", equipo.UsuarioAsignado);
                html.AppendFormat("<td>{0}</td>", equipo.FechaAdquisicion?.ToString("dd/MM/yyyy") ?? "");
                html.AppendFormat("<td>{0}</td>", equipo.Activo ? "Sí" : "No");
                html.Append("</tr>");
            }

            html.Append("</table>");
            html.Append("</body></html>");
            return html.ToString();
        }

        public async Task<byte[]> GenerarHojaAsignacionPDFAsync(int equipoId, CancellationToken ct = default)
        {
            // Este método implementaría la generación de una hoja de asignación para un equipo específico
            // Similar a ExportarPDFAsync, pero con formato de hoja de asignación
            
            // Obtener datos del equipo
            var equipo = await _equipoRepo.ObtenerPorIdAsync(equipoId, ct);
            if (equipo == null)
                throw new ArgumentException("Equipo no encontrado");

            // Generar HTML
            string html = GenerarHTMLHojaAsignacion(equipo);

            // Convertir HTML a PDF
            return System.Text.Encoding.UTF8.GetBytes(html);
        }

        private string GenerarHTMLHojaAsignacion(EquipoComputo equipo)
        {
            var html = new System.Text.StringBuilder();
            html.Append("<html><head>");
            html.Append("<style>");
            html.Append("body { font-family: Arial, sans-serif; }");
            html.Append("table { width: 100%; border-collapse: collapse; }");
            html.Append("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            html.Append("th { background-color: #f2f2f2; }");
            html.Append(".header { text-align: center; margin-bottom: 20px; }");
            html.Append(".firma { margin-top: 50px; border-top: 1px solid black; width: 200px; text-align: center; }");
            html.Append("</style>");
            html.Append("</head><body>");
            html.Append("<div class='header'><h1>Hoja de Asignación de Equipo</h1></div>");
            
            html.Append("<h2>Datos del Equipo</h2>");
            html.Append("<table>");
            html.AppendFormat("<tr><th>Etiqueta</th><td>{0}</td></tr>", equipo.EtiquetaInventario);
            html.AppendFormat("<tr><th>Número Serie</th><td>{0}</td></tr>", equipo.NumeroSerie);
            html.AppendFormat("<tr><th>Tipo</th><td>{0}</td></tr>", equipo.TipoEquipo?.Nombre ?? "");
            html.AppendFormat("<tr><th>Marca</th><td>{0}</td></tr>", equipo.Marca);
            html.AppendFormat("<tr><th>Modelo</th><td>{0}</td></tr>", equipo.Modelo);
            html.AppendFormat("<tr><th>Estado</th><td>{0}</td></tr>", equipo.Estado?.Nombre ?? "");
            html.AppendFormat("<tr><th>Características</th><td>{0}</td></tr>", equipo.Caracteristicas);
            html.Append("</table>");
            
            html.Append("<h2>Ubicación</h2>");
            html.Append("<table>");
            html.AppendFormat("<tr><th>Sede</th><td>{0}</td></tr>", equipo.Zona?.Area?.Sede?.Nombre ?? "");
            html.AppendFormat("<tr><th>Área</th><td>{0}</td></tr>", equipo.Zona?.Area?.Nombre ?? "");
            html.AppendFormat("<tr><th>Zona</th><td>{0}</td></tr>", equipo.Zona?.Nombre ?? "");
            html.Append("</table>");
            
            html.Append("<h2>Usuario Asignado</h2>");
            html.Append("<table>");
            html.AppendFormat("<tr><th>Nombre</th><td>{0}</td></tr>", equipo.Usuario?.NombreCompleto ?? "");
            html.Append("</table>");
            
            html.Append("<div class='firma'>Firma de Recibido</div>");
            
            html.Append("</body></html>");
            return html.ToString();
        }
    }
}