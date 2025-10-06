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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly IEmpleadoService _empleadoService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<TipoEquipo> TiposEquipo { get; } = new();
        public ObservableCollection<Estado> Estados { get; } = new();
        public ObservableCollection<Sede> Sedes { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();
        public ObservableCollection<Empleado> Empleados { get; } = new();

        // Resultados del reporte: DTOs listos para UI/PDF/Excel
        public ObservableCollection<ReporteEquipoDTO> Resultados { get; } = new();

        [ObservableProperty] private TipoEquipo? _tipoEquipoSeleccionado;
        [ObservableProperty] private Estado? _estadoSeleccionado;
        [ObservableProperty] private Sede? _sedeSeleccionada;
        [ObservableProperty] private Area? _areaSeleccionada;
        [ObservableProperty] private Zona? _zonaSeleccionada;

        // Definición explícita para evitar CS0103 (no depender del generador aquí)
        private Empleado? _empleadoSeleccionado;
        public Empleado? EmpleadoSeleccionado
        {
            get => _empleadoSeleccionado;
            set => SetProperty(ref _empleadoSeleccionado, value);
        }

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
            IEmpleadoService empleadoService,
            IDialogService dialogService,
            ILogger<ReportesViewModel> logger)
        {
            _reporteService = reporteService;
            _tipoEquipoService = tipoEquipoService;
            _estadoService = estadoService;
            _sedeService = sedeService;
            _areaService = areaService;
            _zonaService = zonaService;
            _empleadoService = empleadoService;
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

                Empleados.Clear();
                foreach (var emp in await _empleadoService.ObtenerTodosAsync(false)) Empleados.Add(emp);
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
            AreaSeleccionada = null;
            ZonaSeleccionada = null;
            _ = CargarAreasAsync();
        }

        private async Task CargarAreasAsync()
        {
            Areas.Clear();
            Zonas.Clear();

            if (SedeSeleccionada == null)
                return;

            try
            {
                var areas = await _areaService.BuscarAsync(SedeSeleccionada.Id, null, true);
                foreach (var area in areas) Areas.Add(area);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error cargando áreas");
            }
        }

        partial void OnAreaSeleccionadaChanged(Area? value)
        {
            ZonaSeleccionada = null;
            _ = CargarZonasAsync();
        }

        private async Task CargarZonasAsync()
        {
            Zonas.Clear();
            if (AreaSeleccionada == null) return;

            try
            {
                var zonas = await _zonaService.BuscarAsync(AreaSeleccionada.Id, null, true);
                foreach (var zona in zonas) Zonas.Add(zona);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error cargando zonas");
            }
        }

        [RelayCommand]
        public async Task GenerarReporteAsync()
        {
            if (FechaDesde.HasValue && FechaHasta.HasValue && FechaDesde > FechaHasta)
            {
                _dialogService.ShowError("La fecha inicial no puede ser mayor que la fecha final.");
                return;
            }

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
                    // Compatibilidad: UsuarioId = EmpleadoId
                    UsuarioId = EmpleadoSeleccionado?.Id,
                    FechaDesde = FechaDesde,
                    FechaHasta = FechaHasta,
                    IncluirInactivos = IncluirInactivos
                };

                Resultados.Clear();
                var dto = await _reporteService.ObtenerEquiposDTOFiltradosAsync(filtro);
                foreach (var r in dto) Resultados.Add(r);

                if (Resultados.Count == 0)
                    _dialogService.ShowInfo("No se encontraron equipos con los criterios especificados.");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error generando reporte");
                _dialogService.ShowError("Ocurrió un error al generar el reporte.");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public Task LimpiarFiltrosAsync()
        {
            TipoEquipoSeleccionado = null;
            EstadoSeleccionado = null;
            SedeSeleccionada = null;
            AreaSeleccionada = null;
            ZonaSeleccionada = null;
            EmpleadoSeleccionado = null;
            FechaDesde = null;
            FechaHasta = null;
            IncluirInactivos = false;
            Resultados.Clear();
            return Task.CompletedTask;
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
                var bytes = await _reporteService.ExportarExcelAsync(Resultados.ToList());
                await File.WriteAllBytesAsync(save.FileName, bytes);
                _dialogService.ShowInfo("Reporte en Excel generado correctamente.");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error exportando a Excel");
                _dialogService.ShowError("No se pudo exportar a Excel.");
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

            var sfd = new SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = $"Inventario_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            };
            if (sfd.ShowDialog() != true)
                return;

            IsBusy = true;
            try
            {
                var bytes = await _reporteService.ExportarPDFAsync(Resultados.ToList());
                await File.WriteAllBytesAsync(sfd.FileName, bytes);
                _dialogService.ShowInfo("Reporte PDF generado correctamente.");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error exportando PDF");
                _dialogService.ShowError("Ocurrió un error al exportar a PDF.");
            }
            finally { IsBusy = false; }
        }
    }
}