using InventarioComputo.Application.Contracts;
using InventarioComputo.Application.Contracts.Repositories;
using InventarioComputo.Domain.DTOs;
using InventarioComputo.Domain.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
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

        public Task<IReadOnlyList<EquipoComputo>> ObtenerEquiposFiltradosAsync(FiltroReporteDTO filtro, CancellationToken ct = default)
            => _equipoRepo.ObtenerParaReporteAsync(filtro, ct);

        public async Task<byte[]> ExportarExcelAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Inventario Equipos");

            // Encabezados
            ws.Cells[1, 1].Value = "Etiqueta";
            ws.Cells[1, 2].Value = "Número de Serie";
            ws.Cells[1, 3].Value = "Marca";
            ws.Cells[1, 4].Value = "Modelo";
            ws.Cells[1, 5].Value = "Tipo";
            ws.Cells[1, 6].Value = "Estado";
            ws.Cells[1, 7].Value = "Ubicación";
            ws.Cells[1, 8].Value = "Usuario Asignado";
            ws.Cells[1, 9].Value = "Fecha Adquisición";
            ws.Cells[1, 10].Value = "Activo";

            using (var header = ws.Cells[1, 1, 1, 10])
            {
                header.Style.Font.Bold = true;
                header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                header.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                header.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            // Datos
            for (int i = 0; i < datos.Count; i++)
            {
                var r = i + 2;
                var e = datos[i];

                ws.Cells[r, 1].Value = e.EtiquetaInventario;
                ws.Cells[r, 2].Value = e.NumeroSerie;
                ws.Cells[r, 3].Value = e.Marca;
                ws.Cells[r, 4].Value = e.Modelo;
                ws.Cells[r, 5].Value = e.TipoEquipo;
                ws.Cells[r, 6].Value = e.Estado;
                ws.Cells[r, 7].Value = e.Ubicacion;
                ws.Cells[r, 8].Value = e.UsuarioAsignado;

                if (e.FechaAdquisicion.HasValue)
                {
                    ws.Cells[r, 9].Value = e.FechaAdquisicion.Value;
                    ws.Cells[r, 9].Style.Numberformat.Format = "dd/mm/yyyy";
                }

                ws.Cells[r, 10].Value = e.Activo ? "Sí" : "No";
            }

            ws.Cells.AutoFitColumns();
            ws.Cells[1, 1, datos.Count + 1, 10].AutoFilter = true;

            return await package.GetAsByteArrayAsync(ct);
        }

        public Task<byte[]> ExportarPDFAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            if (datos == null) throw new ArgumentNullException(nameof(datos));

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Text("Inventario de Equipos - Henniges Automotive Planta 1")
                            .SemiBold().FontSize(13);

                        row.ConstantItem(120).AlignRight().Text(txt =>
                        {
                            txt.DefaultTextStyle(TextStyle.Default.FontSize(9));
                            txt.Span("Generado: ").SemiBold();
                            txt.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        });
                    });

                    page.Content().PaddingTop(8).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(90);   // Serie
                            columns.ConstantColumn(90);   // Etiqueta
                            columns.RelativeColumn(1);    // Marca
                            columns.RelativeColumn(1);    // Modelo
                            columns.RelativeColumn(1);    // Tipo
                            columns.RelativeColumn(1);    // Estado
                            columns.RelativeColumn(1.2f); // Usuario
                            columns.RelativeColumn(1.6f); // Ubicación
                        });

                        table.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Serie").SemiBold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Etiqueta").SemiBold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Marca").SemiBold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Modelo").SemiBold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Tipo").SemiBold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Estado").SemiBold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Usuario").SemiBold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Ubicación").SemiBold().FontSize(9);
                        });

                        var zebra = false;
                        foreach (var e in datos)
                        {
                            var bg = zebra ? Colors.Grey.Lighten4 : Colors.White;
                            zebra = !zebra;

                            table.Cell().Background(bg).Padding(3).Text(e.NumeroSerie ?? "").FontSize(9);
                            table.Cell().Background(bg).Padding(3).Text(e.EtiquetaInventario ?? "").FontSize(9);
                            table.Cell().Background(bg).Padding(3).Text(e.Marca ?? "").FontSize(9);
                            table.Cell().Background(bg).Padding(3).Text(e.Modelo ?? "").FontSize(9);
                            table.Cell().Background(bg).Padding(3).Text(e.TipoEquipo ?? "").FontSize(9);
                            table.Cell().Background(bg).Padding(3).Text(e.Estado ?? "").FontSize(9);

                            var usuario = e.UsuarioAsignado ?? e.Usuario ?? "";
                            table.Cell().Background(bg).Padding(3).Text(usuario).FontSize(9);

                            var ubic = !string.IsNullOrWhiteSpace(e.Ubicacion)
                                ? e.Ubicacion
                                : string.Join(" / ", new[] { e.Sede, e.Area, e.Zona });
                            table.Cell().Background(bg).Padding(3).Text(ubic).FontSize(9);
                        }

                        table.Cell().ColumnSpan(8).PaddingTop(6).AlignRight()
                             .Text($"Total: {datos.Count} equipos").SemiBold().FontSize(9);
                    });

                    page.Footer().AlignRight().Text(x =>
                    {
                        x.DefaultTextStyle(TextStyle.Default.FontSize(9));
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            var bytes = document.GeneratePdf();
            return Task.FromResult(bytes);
        }
    }
}