using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.Domain.DTOs;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace InventarioComputo.UI.ViewModels
{
    public partial class ReportesViewModel : BaseViewModel
    {
        private readonly IReporteService _reporteService;
        private readonly ITipoEquipoService _tipoEquipoService;
        private readonly IEstadoService _estadoService;
        private readonly ISedeService _sedeService;
        private readonly IAreaService _areaService;
        private readonly IZonaService _zonaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<TipoEquipo> TiposEquipo { get; } = new();
        public ObservableCollection<Estado> Estados { get; } = new();
        public ObservableCollection<Sede> Sedes { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();
        public ObservableCollection<Usuario> Usuarios { get; } = new();

        public ObservableCollection<EquipoComputo> Resultados { get; } = new();

        [ObservableProperty] private TipoEquipo? _tipoEquipoSeleccionado;
        [ObservableProperty] private Estado? _estadoSeleccionado;
        [ObservableProperty] private Sede? _sedeSeleccionada;
        [ObservableProperty] private Area? _areaSeleccionada;
        [ObservableProperty] private Zona? _zonaSeleccionada;
        [ObservableProperty] private Usuario? _usuarioSeleccionado;
        [ObservableProperty] private DateTime? _fechaDesde;
        [ObservableProperty] private DateTime? _fechaHasta;
        [ObservableProperty] private bool _incluirInactivos;

        public ReportesViewModel(
            IReporteService reporteService,
            ITipoEquipoService tipoEquipoService,
            IEstadoService estadoService,
            ISedeService sedeService,
            IAreaService areaService,
            IZonaService zonaService,
            IUsuarioService usuarioService,
            IDialogService dialogService,
            ILogger<ReportesViewModel> logger)
        {
            _reporteService = reporteService;
            _tipoEquipoService = tipoEquipoService;
            _estadoService = estadoService;
            _sedeService = sedeService;
            _areaService = areaService;
            _zonaService = zonaService;
            _usuarioService = usuarioService;
            _dialogService = dialogService;
            Logger = logger;
        }

        [RelayCommand]
        public async Task LoadedAsync() => await CargarCombosAsync();

        private async Task CargarCombosAsync()
        {
            IsBusy = true;
            try
            {
                TiposEquipo.Clear();
                foreach (var t in await _tipoEquipoService.BuscarAsync(null, true)) TiposEquipo.Add(t);

                Estados.Clear();
                foreach (var e in await _estadoService.BuscarAsync(null, true)) Estados.Add(e);

                Sedes.Clear();
                foreach (var s in await _sedeService.BuscarAsync(null, true)) Sedes.Add(s);

                Usuarios.Clear();
                foreach (var u in await _usuarioService.BuscarAsync(null, true)) Usuarios.Add(u);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error cargando catálogos para reportes");
                _dialogService.ShowError("No se pudieron cargar los datos para los filtros del reporte.");
            }
            finally { IsBusy = false; }
        }

        partial void OnSedeSeleccionadaChanged(Sede? value)
        {
            _ = CargarAreasAsync();
            ZonaSeleccionada = null;
        }

        partial void OnAreaSeleccionadaChanged(Area? value) => _ = CargarZonasAsync();

        private async Task CargarAreasAsync()
        {
            Areas.Clear();
            Zonas.Clear();
            if (SedeSeleccionada != null)
            {
                foreach (var a in await _areaService.BuscarAsync(SedeSeleccionada.Id, null, true)) Areas.Add(a);
            }
        }

        private async Task CargarZonasAsync()
        {
            Zonas.Clear();
            if (AreaSeleccionada != null)
            {
                foreach (var z in await _zonaService.BuscarAsync(AreaSeleccionada.Id, null, true)) Zonas.Add(z);
            }
        }

        [RelayCommand]
        public async Task GenerarReporteAsync()
        {
            IsBusy = true;
            try
            {
                var filtro = new FiltroReporteDTO
                {
                    TipoEquipoId = TipoEquipoSeleccionado?.Id,
                    EstadoId = EstadoSeleccionado?.Id,
                    SedeId = SedeSeleccionada?.Id,
                    AreaId = AreaSeleccionada?.Id,
                    ZonaId = ZonaSeleccionada?.Id,
                    UsuarioId = UsuarioSeleccionado?.Id,
                    FechaDesde = FechaDesde,
                    FechaHasta = FechaHasta,
                    IncluirInactivos = IncluirInactivos
                };

                Resultados.Clear();
                var equipos = await _reporteService.ObtenerEquiposFiltradosAsync(filtro);
                foreach (var e in equipos) Resultados.Add(e);

                if (Resultados.Count == 0)
                    _dialogService.ShowInfo("No se encontraron equipos con los criterios especificados.");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error generando reporte");
                _dialogService.ShowError("Ocurrió un error al generar el reporte: " + ex.Message);
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task LimpiarFiltrosAsync()
        {
            TipoEquipoSeleccionado = null;
            EstadoSeleccionado = null;
            SedeSeleccionada = null;
            AreaSeleccionada = null;
            ZonaSeleccionada = null;
            UsuarioSeleccionado = null;
            FechaDesde = null;
            FechaHasta = null;
            IncluirInactivos = false;
            Resultados.Clear();
        }

        [RelayCommand]
        public async Task ExportarExcelAsync()
        {
            if (Resultados.Count == 0)
            {
                _dialogService.ShowInfo("No hay datos para exportar. Genere un reporte primero.");
                return;
            }

            var save = new SaveFileDialog
            {
                Title = "Guardar reporte en Excel",
                Filter = "Excel (*.xlsx)|*.xlsx",
                FileName = $"Inventario_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };

            if (save.ShowDialog() != true)
                return;

            IsBusy = true;
            try
            {
                var dto = MapearResultadosADTO(Resultados);
                var bytes = await _reporteService.ExportarExcelAsync(dto);
                await File.WriteAllBytesAsync(save.FileName, bytes);
                _dialogService.ShowInfo("Reporte en Excel generado correctamente.");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error exportando a Excel");
                _dialogService.ShowError("No se pudo exportar a Excel: " + ex.Message);
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task ExportarPdfAsync()
        {
            if (Resultados.Count == 0)
            {
                _dialogService.ShowInfo("No hay datos para exportar. Genere un reporte primero.");
                return;
            }

            try
            {
                var datos = MapearResultadosADTO(Resultados);

                var sfd = new SaveFileDialog
                {
                    Filter = "PDF (*.pdf)|*.pdf",
                    FileName = $"Inventario_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
                };
                if (sfd.ShowDialog() == true)
                {
                    var bytes = await _reporteService.ExportarPDFAsync(datos);
                    await File.WriteAllBytesAsync(sfd.FileName, bytes);
                    _dialogService.ShowInfo("Reporte PDF generado correctamente.");
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error exportando PDF");
                _dialogService.ShowError("Ocurrió un error al exportar a PDF.");
            }
        }

        private static List<ReporteEquipoDTO> MapearResultadosADTO(ObservableCollection<EquipoComputo> equipos)
        {
            var list = new List<ReporteEquipoDTO>(equipos.Count);
            foreach (var e in equipos)
            {
                string ubicacion = string.Empty;
                if (e.Zona != null)
                {
                    var sede = e.Zona.Area?.Sede?.Nombre ?? "";
                    var area = e.Zona.Area?.Nombre ?? "";
                    var zona = e.Zona.Nombre ?? "";
                    ubicacion = string.Join(" > ", new[] { sede, area, zona }.Where(x => !string.IsNullOrWhiteSpace(x)));
                }

                list.Add(new ReporteEquipoDTO
                {
                    EtiquetaInventario = e.EtiquetaInventario,
                    NumeroSerie = e.NumeroSerie,
                    Marca = e.Marca,
                    Modelo = e.Modelo,
                    TipoEquipo = e.TipoEquipo?.Nombre,
                    Estado = e.Estado?.Nombre,
                    Ubicacion = ubicacion,
                    UsuarioAsignado = e.Usuario?.NombreCompleto,
                    FechaAdquisicion = e.FechaAdquisicion.HasValue ? e.FechaAdquisicion.Value : DateTime.Now,
                    Activo = e.Activo
                });
            }
            return list;
        }
    }
}