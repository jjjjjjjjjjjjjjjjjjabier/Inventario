using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventarioComputo.Application.Contracts;
using InventarioComputo.Domain.Entities;
using InventarioComputo.Domain.DTOs;
using InventarioComputo.UI.Services;
using InventarioComputo.UI.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly IUsuarioService _usuarioService;
        private readonly IDialogService _dialogService;

        // Colecciones para combos
        public ObservableCollection<TipoEquipo> TiposEquipo { get; } = new();
        public ObservableCollection<Estado> Estados { get; } = new();
        public ObservableCollection<Sede> Sedes { get; } = new();
        public ObservableCollection<Area> Areas { get; } = new();
        public ObservableCollection<Zona> Zonas { get; } = new();
        public ObservableCollection<Usuario> Usuarios { get; } = new();

        // Colección para resultados
        public ObservableCollection<EquipoComputo> Resultados { get; } = new();

        // Propiedades para elementos seleccionados en filtros
        [ObservableProperty] private TipoEquipo? _tipoEquipoSeleccionado;
        [ObservableProperty] private Estado? _estadoSeleccionado;
        [ObservableProperty] private Sede? _sedeSeleccionada;
        [ObservableProperty] private Area? _areaSeleccionada;
        [ObservableProperty] private Zona? _zonaSeleccionada;
        [ObservableProperty] private Usuario? _usuarioSeleccionado;

        // Propiedades para fechas
        [ObservableProperty] private DateTime? _fechaDesde;
        [ObservableProperty] private DateTime? _fechaHasta;

        // Propiedad para incluir inactivos
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
        public async Task LoadedAsync()
        {
            await CargarCombosAsync();
        }

        private async Task CargarCombosAsync()
        {
            IsBusy = true;
            try
            {
                // Cargar todos los catálogos para filtros
                TiposEquipo.Clear();
                var tipos = await _tipoEquipoService.BuscarAsync(null, true);
                foreach (var t in tipos) TiposEquipo.Add(t);

                Estados.Clear();
                var estados = await _estadoService.BuscarAsync(null, true);
                foreach (var e in estados) Estados.Add(e);

                Sedes.Clear();
                var sedes = await _sedeService.BuscarAsync(null, true);
                foreach (var s in sedes) Sedes.Add(s);

                Usuarios.Clear();
                var usuarios = await _usuarioService.BuscarAsync(null, true);
                foreach (var u in usuarios) Usuarios.Add(u);
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

        partial void OnAreaSeleccionadaChanged(Area? value)
        {
            _ = CargarZonasAsync();
        }

        private async Task CargarAreasAsync()
        {
            Areas.Clear();
            Zonas.Clear();
            if (SedeSeleccionada != null)
            {
                var areas = await _areaService.BuscarAsync(SedeSeleccionada.Id, null, true);
                foreach (var a in areas) Areas.Add(a);
            }
        }

        private async Task CargarZonasAsync()
        {
            Zonas.Clear();
            if (AreaSeleccionada != null)
            {
                var zonas = await _zonaService.BuscarAsync(AreaSeleccionada.Id, null, true);
                foreach (var z in zonas) Zonas.Add(z);
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
                foreach (var equipo in equipos)
                {
                    Resultados.Add(equipo);
                }

                if (Resultados.Count == 0)
                {
                    _dialogService.ShowInfo("No se encontraron equipos con los criterios especificados.");
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error generando reporte");
                _dialogService.ShowError("Ocurrió un error al generar el reporte: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
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

            IsBusy = true;
            try
            {
                // Aquí implementar la lógica de exportación a Excel 
                // usando EPPlus o librería similar
                _dialogService.ShowInfo("Exportación a Excel no implementada aún.");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error exportando a Excel");
                _dialogService.ShowError("No se pudo exportar a Excel: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ExportarPDFAsync()
        {
            if (Resultados.Count == 0)
            {
                _dialogService.ShowInfo("No hay datos para exportar. Genere un reporte primero.");
                return;
            }

            IsBusy = true;
            try
            {
                // Aquí implementar la lógica de exportación a PDF
                // usando QuestPDF o librería similar
                _dialogService.ShowInfo("Exportación a PDF no implementada aún.");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error exportando a PDF");
                _dialogService.ShowError("No se pudo exportar a PDF: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}