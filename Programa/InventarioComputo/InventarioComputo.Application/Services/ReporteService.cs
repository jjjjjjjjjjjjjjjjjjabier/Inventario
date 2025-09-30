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
            var now = DateTime.Now;

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Text("Inventario de Equipos de Cómputo").SemiBold().FontSize(16);
                        row.ConstantItem(200).AlignRight().Text($"Fecha: {now:dd/MM/yyyy HH:mm}");
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);  // Etiqueta
                            columns.RelativeColumn(2);  // Núm. Serie
                            columns.RelativeColumn(2);  // Marca
                            columns.RelativeColumn(2);  // Modelo
                            columns.RelativeColumn(2);  // Tipo
                            columns.RelativeColumn(2);  // Estado
                            columns.RelativeColumn(3);  // Usuario
                            columns.RelativeColumn(4);  // Ubicación
                            columns.RelativeColumn(2);  // Fecha Adq.
                            columns.RelativeColumn(1);  // Activo
                        });

                        void Header(string t) => table.Cell().Element(h => h
                            .PaddingVertical(4).PaddingHorizontal(3)
                            .Background(Colors.Grey.Lighten3)
                            .Border(1).BorderColor(Colors.Grey.Lighten2))
                            .Text(t).SemiBold();

                        Header("Etiqueta");
                        Header("Núm. Serie");
                        Header("Marca");
                        Header("Modelo");
                        Header("Tipo");
                        Header("Estado");
                        Header("Usuario");
                        Header("Ubicación");
                        Header("Fecha Adq.");
                        Header("Activo");

                        foreach (var e in datos)
                        {
                            Cell(e.EtiquetaInventario);
                            Cell(e.NumeroSerie);
                            Cell(e.Marca);
                            Cell(e.Modelo);
                            Cell(e.TipoEquipo);
                            Cell(e.Estado);
                            Cell(e.UsuarioAsignado ?? "-");
                            Cell(string.IsNullOrWhiteSpace(e.Ubicacion) ? "-" : e.Ubicacion);
                            Cell(e.FechaAdquisicion?.ToString("dd/MM/yyyy") ?? "-");
                            Cell(e.Activo ? "Sí" : "No");
                        }

                        void Cell(string? t) => table.Cell().Element(b => b
                            .PaddingVertical(2).PaddingHorizontal(3)
                            .BorderBottom(1).BorderColor(Colors.Grey.Lighten3))
                            .Text(t ?? string.Empty);
                    });

                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();

            return Task.FromResult(pdfBytes);
        }
    }
}