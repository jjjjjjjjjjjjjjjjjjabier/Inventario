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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrawingColor = System.Drawing.Color;

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

        public async Task<IReadOnlyList<ReporteEquipoDTO>> ObtenerEquiposDTOFiltradosAsync(FiltroReporteDTO filtro, CancellationToken ct = default)
        {
            var equipos = await _equipoRepo.ObtenerParaReporteAsync(filtro, ct);

            static (string Sede, string Area, string Zona, string Ubicacion) BuildUbicacion(EquipoComputo e)
            {
                // Preferir cadena vía Zona
                var sede = e.Zona?.Area?.Sede?.Nombre
                           ?? e.Area?.Sede?.Nombre
                           ?? e.Sede?.Nombre
                           ?? string.Empty;

                var area = e.Zona?.Area?.Nombre
                           ?? e.Area?.Nombre
                           ?? string.Empty;

                var zona = e.Zona?.Nombre ?? string.Empty;

                var partes = new List<string>();
                if (!string.IsNullOrWhiteSpace(sede)) partes.Add(sede);
                if (!string.IsNullOrWhiteSpace(area)) partes.Add(area);
                if (!string.IsNullOrWhiteSpace(zona)) partes.Add(zona);

                return (sede, area, zona, string.Join(" / ", partes));
            }

            return equipos.Select(e =>
            {
                var ub = BuildUbicacion(e);
                return new ReporteEquipoDTO
                {
                    Id = e.Id,
                    NumeroSerie = e.NumeroSerie,
                    EtiquetaInventario = e.EtiquetaInventario,
                    Marca = e.Marca,
                    Modelo = e.Modelo,
                    TipoEquipo = e.TipoEquipo?.Nombre ?? "",
                    Estado = e.Estado?.Nombre ?? "",
                    Usuario = e.Empleado?.NombreCompleto ?? "",
                    UsuarioAsignado = e.Empleado?.NombreCompleto ?? "",
                    Sede = ub.Sede,
                    Area = ub.Area,
                    Zona = ub.Zona,
                    Ubicacion = ub.Ubicacion,
                    FechaAdquisicion = e.FechaAdquisicion ?? DateTime.MinValue,
                    Costo = e.Costo, // se conserva en DTO por compatibilidad, NO se exporta
                    Activo = e.Activo
                };
            }).ToList();
        }

        public Task<byte[]> ExportarExcelAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("Inventario");

            var headers = new[]
            {
                "Num. Serie","Etiqueta","Marca","Modelo","Tipo","Estado",
                "Asignado a","Ubicación","Sede","Área","Zona","Fecha Adquisición","Activo"
            };

            for (int i = 0; i < headers.Length; i++)
                ws.Cells[1, i + 1].Value = headers[i];

            using (var rng = ws.Cells[1, 1, 1, headers.Length])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(DrawingColor.LightGray);
            }

            int row = 2;
            foreach (var d in datos)
            {
                ws.Cells[row, 1].Value = d.NumeroSerie;
                ws.Cells[row, 2].Value = d.EtiquetaInventario;
                ws.Cells[row, 3].Value = d.Marca;
                ws.Cells[row, 4].Value = d.Modelo;
                ws.Cells[row, 5].Value = d.TipoEquipo;
                ws.Cells[row, 6].Value = d.Estado;
                ws.Cells[row, 7].Value = d.UsuarioAsignado;
                ws.Cells[row, 8].Value = d.Ubicacion;
                ws.Cells[row, 9].Value = d.Sede;
                ws.Cells[row,10].Value = d.Area;
                ws.Cells[row,11].Value = d.Zona;
                ws.Cells[row,12].Value = d.FechaAdquisicion == DateTime.MinValue ? null : d.FechaAdquisicion;
                ws.Cells[row,12].Style.Numberformat.Format = "yyyy-mm-dd";
                ws.Cells[row,13].Value = d.Activo ? "Sí" : "No";
                row++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return Task.FromResult(p.GetAsByteArray());
        }

        public Task<byte[]> ExportarPDFAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Header().Text($"Inventario de Equipos - {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .SemiBold().FontSize(14).FontColor(Colors.Blue.Medium);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(85);
                            c.ConstantColumn(75);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1.2f);
                            c.RelativeColumn(1.6f);
                            c.ConstantColumn(75);
                            c.ConstantColumn(50);
                        });

                        void Header(string t) =>
                            table.Cell().Element(e =>
                                e.DefaultTextStyle(x => x.SemiBold()).Padding(4)
                                 .Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Grey.Lighten1))
                               .Text(t).FontSize(9);

                        Header("Serie");
                        Header("Etiqueta");
                        Header("Marca");
                        Header("Modelo");
                        Header("Tipo");
                        Header("Estado");
                        Header("Asignado a");
                        Header("Ubicación");
                        Header("Fecha");
                        Header("Activo");

                        foreach (var d in datos)
                        {
                            void Cell(string t) =>
                                table.Cell().Element(e =>
                                    e.PaddingVertical(3).PaddingHorizontal(2)
                                     .BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3))
                                   .Text(t).FontSize(9);

                            Cell(d.NumeroSerie);
                            Cell(d.EtiquetaInventario);
                            Cell(d.Marca);
                            Cell(d.Modelo);
                            Cell(d.TipoEquipo);
                            Cell(d.Estado);
                            Cell(d.UsuarioAsignado);
                            Cell(d.Ubicacion);
                            Cell(d.FechaAdquisicion == DateTime.MinValue ? "" : d.FechaAdquisicion.ToString("yyyy-MM-dd"));
                            Cell(d.Activo ? "Sí" : "No");
                        }
                    });

                    page.Footer().AlignRight().Text(x =>
                    {
                        x.DefaultTextStyle(ts => ts.FontSize(9));
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            var bytes = doc.GeneratePdf();
            return Task.FromResult(bytes);
        }
    }
}