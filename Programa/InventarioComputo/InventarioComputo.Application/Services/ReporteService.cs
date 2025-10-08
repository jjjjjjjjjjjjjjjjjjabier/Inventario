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
using System.IO;
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
                    Costo = e.Costo,
                    Activo = e.Activo
                };
            }).ToList();
        }

        public Task<byte[]> ExportarExcelAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("Inventario");

            // Configurar anchuras de columna antes de empezar
            ws.Column(1).Width = 15;  // Ancho para número de serie
            
            // Espaciado mejorado - Dejar espacio para logo y encabezados
            ws.Row(1).Height = 35;    // Altura suficiente para el logo
            ws.Row(2).Height = 22;    // Altura para la línea de fecha/total
            ws.Row(3).Height = 15;    // Espacio de separación

            // Añadir logo y título con mejor posicionamiento
            var logoBytes = TryLoadLogo();
            if (logoBytes != null)
            {
                using (var ms = new MemoryStream(logoBytes))
                {
                    var picture = ws.Drawings.AddPicture("Logo", ms);
                    // Posicionar en A1 con margen adecuado
                    picture.SetPosition(0, 10, 0, 10);
                    // Tamaño controlado para evitar que invada otras celdas
                    picture.SetSize(30);
                }
            }

            // Título con mejor formateo - celda C1
            ws.Cells[1, 3].Value = "Inventario de Equipos";
            ws.Cells[1, 3, 1, 8].Merge = true;
            ws.Cells[1, 3].Style.Font.Size = 16;
            ws.Cells[1, 3].Style.Font.Bold = true;
            ws.Cells[1, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            // Fecha y total en fila 2 con mejor alineación
            ws.Cells[2, 3].Value = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm} • Total: {datos.Count}";
            ws.Cells[2, 3, 2, 8].Merge = true;
            ws.Cells[2, 3].Style.Font.Size = 11;
            ws.Cells[2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            // Encabezados de columnas empiezan en fila 4 con altura explícita
            ws.Row(4).Height = 20;
            var headers = new[]
            {
                "Num. Serie","Etiqueta","Marca","Modelo","Tipo","Estado",
                "Asignado a","Ubicación","Sede","Área","Zona","Fecha Adquisición","Activo"
            };

            // Empezar encabezados en fila 4
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[4, i + 1].Value = headers[i];
                // Garantizar que cada columna tenga un ancho mínimo razonable
                if (i > 0) // No ajustar la primera columna que ya configuramos
                    ws.Column(i + 1).Width = Math.Max(ws.Column(i + 1).Width, 12);
            }

            using (var rng = ws.Cells[4, 1, 4, headers.Length])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(DrawingColor.LightGray);
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Datos empiezan en fila 5
            int row = 5;
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
                ws.Cells[row, 10].Value = d.Area;
                ws.Cells[row, 11].Value = d.Zona;
                ws.Cells[row, 12].Value = d.FechaAdquisicion == DateTime.MinValue ? null : d.FechaAdquisicion;
                ws.Cells[row, 12].Style.Numberformat.Format = "yyyy-mm-dd";
                ws.Cells[row, 13].Value = d.Activo ? "Sí" : "No";
                row++;
            }

            // Ajustar automáticamente el ancho de las columnas al contenido
            // pero con un límite máximo para evitar columnas demasiado anchas
            ws.Cells[ws.Dimension.Address].AutoFitColumns(15, 50); // Mín 15, máx 50
            
            // Ajustar las celdas para una mejor presentación
            using (var dataRange = ws.Cells[5, 1, row - 1, headers.Length])
            {
                dataRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }

            return Task.FromResult(p.GetAsByteArray());
        }

        // PDF en formato tabla (A4 apaisado), robusto a saltos de página
        public Task<byte[]> ExportarPDFAsync(IReadOnlyList<ReporteEquipoDTO> datos, CancellationToken ct = default)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            
            try
            {
                // Habilitamos depuración para diagnósticos
                QuestPDF.Settings.EnableDebugging = true;
                
                var logoBytes = TryLoadLogo();
                datos ??= Array.Empty<ReporteEquipoDTO>();

                var doc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(25);
                        page.PageColor(Colors.White);

                        // Configuración de texto más robusta
                        page.DefaultTextStyle(t => t
                            .FontSize(8)
                            .WrapAnywhere(true)
                            .FontColor(Colors.Black));

                        // Encabezado simplificado
                        page.Header().Element(header =>
                        {
                            header.Row(row =>
                            {
                                // Logo con ancho adecuado
                                if (logoBytes != null)
                                {
                                    row.ConstantItem(80).Height(35).AlignMiddle().Element(e =>
                                        e.Image(logoBytes).FitArea()
                                    );

                                    // Agregar espacio entre el logo y el título
                                    row.ConstantItem(20);
                                }

                                // Contenido del título mejor posicionado
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().AlignLeft().Text("Inventario de Equipos").SemiBold().FontSize(14);
                                    col.Item().AlignLeft().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm} • Total: {datos.Count}").FontSize(9);
                                });
                            });
                        });

                        // Tabla con dimensiones ajustadas
                        page.Content().PaddingVertical(10).Element(e =>
                        {
                            e.Table(table =>
                            {
                                // Definición de columnas más equilibrada
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.ConstantColumn(80);    // Serie
                                    cols.ConstantColumn(60);    // Etiqueta
                                    cols.ConstantColumn(60);    // Marca
                                    cols.ConstantColumn(70);    // Modelo
                                    cols.ConstantColumn(60);    // Tipo
                                    cols.ConstantColumn(60);    // Estado
                                    cols.ConstantColumn(90);    // Asignado
                                    cols.RelativeColumn(2);     // Ubicación
                                    cols.ConstantColumn(70);    // Fecha
                                    cols.ConstantColumn(40);    // Activo
                                });

                                // Estilo para celdas de encabezado
                                IContainer HeaderCell(IContainer c) => c
                                    .Border(0.5f)
                                    .BorderColor(Colors.Grey.Medium)
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(3)
                                    .AlignMiddle()
                                    .AlignCenter();

                                // Estilo para celdas de datos
                                IContainer DataCell(IContainer c) => c
                                    .Border(0.2f)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(3);

                                // Encabezados
                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderCell).Text("Num. Serie");
                                    header.Cell().Element(HeaderCell).Text("Etiqueta");
                                    header.Cell().Element(HeaderCell).Text("Marca");
                                    header.Cell().Element(HeaderCell).Text("Modelo");
                                    header.Cell().Element(HeaderCell).Text("Tipo");
                                    header.Cell().Element(HeaderCell).Text("Estado");
                                    header.Cell().Element(HeaderCell).Text("Asignado a");
                                    header.Cell().Element(HeaderCell).Text("Ubicación");
                                    header.Cell().Element(HeaderCell).Text("Adquisición");
                                    header.Cell().Element(HeaderCell).Text("Activo");
                                });

                                // Filas de datos con manejo seguro
                                foreach (var d in datos)
                                {
                                    // Garantizar que textos largos puedan partirse
                                    string SafeText(string? text) => 
                                        (text?.Length > 100) ? text.Substring(0, 97) + "..." : text ?? "";
                                    
                                    var fecha = d.FechaAdquisicion == DateTime.MinValue ? 
                                        "" : d.FechaAdquisicion.ToString("yyyy-MM-dd");

                                    table.Cell().Element(DataCell).Text(SafeText(d.NumeroSerie));
                                    table.Cell().Element(DataCell).Text(SafeText(d.EtiquetaInventario));
                                    table.Cell().Element(DataCell).Text(SafeText(d.Marca));
                                    table.Cell().Element(DataCell).Text(SafeText(d.Modelo));
                                    table.Cell().Element(DataCell).Text(SafeText(d.TipoEquipo));
                                    table.Cell().Element(DataCell).Text(SafeText(d.Estado));
                                    table.Cell().Element(DataCell).Text(SafeText(d.UsuarioAsignado));
                                    
                                    // Ubicación con manejo especial para textos largos
                                    table.Cell().Element(DataCell).Element(c =>
                                    {
                                        string ubicText = SafeText(d.Ubicacion).Replace("/", " / ");
                                        c.Text(ubicText).WrapAnywhere();
                                    });
                                    
                                    table.Cell().Element(DataCell).Text(fecha);
                                    table.Cell().Element(DataCell).Text(d.Activo ? "Sí" : "No");
                                }
                            });
                        });

                        // Pie de página simple
                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Página ").FontSize(9);
                            text.CurrentPageNumber().FontSize(9);
                            text.Span(" de ").FontSize(9);
                            text.TotalPages().FontSize(9);
                        });
                    });
                });

                return Task.FromResult(doc.GeneratePdf());
            }
            catch (Exception ex)
            {
                // Loguear el error para diagnóstico
                System.Diagnostics.Debug.WriteLine($"Error generando PDF: {ex}");
                
                // Crear un documento de error simple como alternativa
                var docError = Document.Create(container =>
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(40);
                        page.Content().Column(col =>
                        {
                            col.Item().Text("Error al generar reporte").FontSize(14).FontColor(Colors.Red.Medium);
                            col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);
                            col.Item().Height(10);
                            col.Item().Text("Se ha producido un error al generar el reporte. " +
                                "Esto puede deberse a un problema con los datos o el formato.").FontSize(10);
                            col.Item().Text("Por favor, contacte al administrador del sistema.").FontSize(10);
                            col.Item().Height(10);
                            // Agregar más detalles del error para debugging
                            if (System.Diagnostics.Debugger.IsAttached)
                            {
                                col.Item().Text("Detalles del error (solo en modo debug):").FontSize(8);
                                col.Item().Text(ex.ToString()).FontSize(7);
                            }
                        });
                    }));
            
                return Task.FromResult(docError.GeneratePdf());
            }
        }

        // Carga el logo desde la carpeta de salida
        private static byte[]? TryLoadLogo()
        {
            try
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png");
                if (File.Exists(path))
                    return File.ReadAllBytes(path);

                var alt = Path.Combine(AppContext.BaseDirectory, "logo.png");
                if (File.Exists(alt))
                    return File.ReadAllBytes(alt);
            }
            catch
            {
                // Silenciosamente fallamos si no podemos cargar el logo
            }
            return null;
        }
    }
}